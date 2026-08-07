(function () {
  'use strict';

  const reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

/* 相伴计时器 */
  const together = document.querySelector('.together[data-love-start]');
  if (together) {
    const start = new Date(together.dataset.loveStart.replace(' ', 'T')).getTime();
    const fields = Object.fromEntries(
      ['days', 'hours', 'minutes', 'seconds'].map((key) => [key, together.querySelector(`[data-time="${key}"]`)])
    );
    if (!Number.isNaN(start)) {
      const update = () => {
        const distance = Math.max(0, Date.now() - start);
        const values = {
          days: Math.floor(distance / 86400000),
          hours: Math.floor(distance / 3600000) % 24,
          minutes: Math.floor(distance / 60000) % 60,
          seconds: Math.floor(distance / 1000) % 60
        };
        Object.keys(values).forEach((key) => {
          if (fields[key]) {
            fields[key].textContent = key === 'days' ? String(values[key]) : String(values[key]).padStart(2, '0');
          }
        });
      };
      update();
      window.setInterval(update, 1000);
    }
  }

  /* 首页中间的爱心：点一下就是一次心动，身份由服务端认，前端只负责好看 */
  const heart = document.querySelector('[data-heart]');
  if (heart) {
    const mark = heart.closest('.love-mark');
    const endpoint = heart.dataset.heartEndpoint;
    const token = heart.dataset.heartToken;
    const role = heart.dataset.heartRole;
    let dailyLimit = parseInt(heart.dataset.heartLimit, 10) || 99;
    let selfToday = parseInt(heart.dataset.heartSelfToday, 10) || 0;
    const defaultHeartLabel = heart.getAttribute('aria-label') || '送出一颗心';
    const defaultHeartTitle = heart.title;

    const counters = {};
    document.querySelectorAll('[data-heart-count]').forEach((el) => {
      counters[el.dataset.heartCount] = el.querySelector('strong');
    });
    const stats = {};
    document.querySelectorAll('[data-heart-stat]').forEach((el) => {
      stats[el.dataset.heartStat] = el;
    });

    const compactNumbers = window.matchMedia('(max-width: 800px)');
    const num = (el) => {
      if (!el) return 0;
      const raw = el.dataset.value !== undefined ? el.dataset.value : el.textContent.replace(/\D/g, '');
      return parseInt(raw, 10) || 0;
    };
    const formatHeartNumber = (value) => {
      const safe = Math.max(0, parseInt(value, 10) || 0);
      const ceiling = compactNumbers.matches ? 9999 : 99999;
      return safe > ceiling ? ceiling + '+' : String(safe);
    };
    const put = (el, value) => {
      if (!el) return;
      const safe = Math.max(0, parseInt(value, 10) || 0);
      el.dataset.value = String(safe);
      el.textContent = formatHeartNumber(safe);
    };
    const allHeartNumbers = () => Object.values(counters).concat(Object.values(stats)).filter(Boolean);
    allHeartNumbers().forEach((el) => put(el, num(el)));
    const refreshHeartNumbers = () => allHeartNumbers().forEach((el) => put(el, num(el)));
    if (compactNumbers.addEventListener) compactNumbers.addEventListener('change', refreshHeartNumbers);
    else if (compactNumbers.addListener) compactNumbers.addListener(refreshHeartNumbers);
    const bump = (el) => {
      if (!el) return;
      el.classList.remove('is-bumped');
      void el.offsetWidth;
      el.classList.add('is-bumped');
    };

    const feedback = document.createElement('span');
    const touchLike = window.matchMedia('(hover: none)').matches;
    feedback.className = 'heart-feedback' + (touchLike ? ' is-on is-hint' : '');
    feedback.setAttribute('role', 'status');
    feedback.setAttribute('aria-live', 'polite');
    feedback.textContent = touchLike ? '轻点一次 · 长按连续传递' : '';
    mark.appendChild(feedback);

    let queue = [];
    let timer = 0;
    let comboCount = 0;
    let comboAt = 0;
    let feedbackTimer = 0;
    let warned = false;
    let sending = false;
    let holdDelay = 0;
    let holdRepeat = 0;
    let holdActive = false;
    let suppressClick = false;
    let celebrating = false;
    let beatDuration = 450;

    const isAtLimit = () => selfToday >= dailyLimit;
    const limitFeedbackText = () => role === 'guest' ? '今日祝福已达上限' : '今日想你已达上限';

    const showLimitFeedback = () => {
      if (celebrating) return;
      window.clearTimeout(feedbackTimer);
      feedback.className = 'heart-feedback is-on is-capped';
      feedback.textContent = limitFeedbackText();
      feedback.classList.remove('is-reminding');
      void feedback.offsetWidth;
      feedback.classList.add('is-reminding');
      feedbackTimer = window.setTimeout(() => feedback.classList.remove('is-reminding'), 260);
    };

    const setLimitState = () => {
      const capped = isAtLimit();
      // 原生 disabled 不会触发移动端点击，因此用 aria-disabled 做“逻辑禁用”：
      // 不再计数，但仍允许点按查看明确的上限提示。
      heart.disabled = false;
      heart.classList.toggle('is-capped', capped);
      heart.setAttribute('aria-disabled', capped ? 'true' : 'false');
      if (capped) {
        window.clearTimeout(holdDelay);
        window.clearTimeout(holdRepeat);
        holdActive = false;
        mark.classList.remove('is-holding');
        const cappedLabel = role === 'guest' ? '今天的 99 份祝福已经全部送达' : '今天的 99 次心动已经全部送达';
        heart.setAttribute('aria-label', cappedLabel);
        heart.title = cappedLabel + '，明天再继续';
        showLimitFeedback();
      } else {
        heart.setAttribute('aria-label', defaultHeartLabel);
        heart.title = defaultHeartTitle;
        if (feedback.classList.contains('is-capped')) {
          feedback.className = 'heart-feedback' + (touchLike ? ' is-on is-hint' : '');
          feedback.textContent = touchLike ? '轻点一次 · 长按连续传递' : '';
        }
      }
      return capped;
    };

    const celebrate99 = () => {
      if (document.querySelector('.heart-celebration')) return;
      celebrating = true;
      window.clearTimeout(feedbackTimer);
      feedback.className = 'heart-feedback';
      feedback.textContent = '';
      bump(stats.today);
      stats.today.classList.add('is-milestone');

      const layer = document.createElement('div');
      layer.className = 'heart-celebration';
      layer.setAttribute('aria-hidden', 'true');
      const colors = ['#ef91a8', '#f5bf75', '#d7b3ff', '#ffffff', '#f7dbe3'];
      const origins = [[23, 33], [50, 24], [77, 35], [35, 67], [68, 66]];
      origins.forEach((origin, burst) => {
        for (let index = 0; index < 15; index++) {
          const angle = (Math.PI * 2 * index / 15) + Math.random() * .2;
          const distance = 70 + Math.random() * 105;
          const particle = document.createElement('span');
          particle.className = 'celebration-particle';
          particle.textContent = index % 4 === 0 ? '♥' : '✦';
          particle.style.left = origin[0] + '%';
          particle.style.top = origin[1] + '%';
          particle.style.setProperty('--particle-x', (Math.cos(angle) * distance).toFixed(1) + 'px');
          particle.style.setProperty('--particle-y', (Math.sin(angle) * distance).toFixed(1) + 'px');
          particle.style.setProperty('--particle-rotate', (Math.random() * 360 - 180).toFixed(0) + 'deg');
          particle.style.setProperty('--particle-delay', (burst * .13 + Math.random() * .12).toFixed(2) + 's');
          particle.style.setProperty('--particle-size', (8 + Math.random() * 9).toFixed(0) + 'px');
          particle.style.setProperty('--particle-color', colors[(index + burst) % colors.length]);
          layer.appendChild(particle);
        }
      });

      const banner = document.createElement('div');
      banner.className = 'celebration-banner';
      banner.innerHTML = role === 'guest'
        ? '99 份祝福 · 愿你们长长久久<small>BEST WISHES FOR YOU</small>'
        : '99 次心动 · 爱你长长久久<small>LOVE WITHOUT END</small>';
      layer.appendChild(banner);
      document.body.appendChild(layer);
      window.setTimeout(() => {
        layer.remove();
        celebrating = false;
        showLimitFeedback();
      }, 3800);
    };

    /* 心跳一下 + 飘一颗小爱心，连点时的连击数显示在旁边。
       动画结束要把 is-beating 摘掉，否则那颗心就不再自己呼吸了 */
    let beatTimer = 0;
    const beat = () => {
      heart.classList.remove('is-beating');
      void heart.offsetWidth;
      heart.classList.add('is-beating');
      window.clearTimeout(beatTimer);
      beatTimer = window.setTimeout(() => heart.classList.remove('is-beating'), beatDuration + 90);

      if (reduceMotion || mark.querySelectorAll('.heart-spark').length > 16) return;

      const spark = document.createElement('span');
      spark.className = 'heart-spark';
      spark.style.setProperty('--drift', (Math.random() * 72 - 36).toFixed(1) + 'px');
      spark.style.setProperty('--tilt', (Math.random() * 50 - 25).toFixed(1) + 'deg');
      spark.style.setProperty('--size', (0.65 + Math.random() * 0.5).toFixed(2));
      spark.innerHTML = '<svg class="icon" aria-hidden="true"><use href="#i-heart"></use></svg>';
      spark.addEventListener('animationend', () => spark.remove());
      // 页面在后台时动画不会推进，animationend 也就不会来，兜底删掉
      window.setTimeout(() => spark.remove(), 2000);
      mark.appendChild(spark);
    };

    const applyComboPace = () => {
      const step = Math.min(Math.max(comboCount - 1, 0), 18);
      beatDuration = Math.max(170, 420 - step * 14);
      const bumpDuration = Math.max(150, 360 - step * 11);
      heart.style.setProperty('--beat-duration', beatDuration + 'ms');
      [counters[role], stats.today].forEach((el) => {
        if (el) el.style.setProperty('--bump-duration', bumpDuration + 'ms');
      });
    };

    const clearComboFeedback = () => {
      if (isAtLimit()) {
        showLimitFeedback();
        return;
      }
      if (touchLike) {
        feedback.className = 'heart-feedback is-on is-hint';
        feedback.textContent = '轻点一次 · 长按连续传递';
      } else {
        feedback.className = 'heart-feedback';
        feedback.textContent = '';
      }
    };

    const showCombo = (source) => {
      feedback.className = 'heart-feedback is-on';
      if (source === 'hold') {
        feedback.textContent = (role === 'guest' ? '长按祝福 · ' : '长按想你 · ') + '×' + comboCount;
      } else if (comboCount >= 2) {
        feedback.textContent = (role === 'guest' ? '祝福连击 · ' : '想你连击 · ') + '×' + comboCount;
      } else {
        feedback.textContent = role === 'guest'
          ? '第 ' + num(stats.today) + ' 份祝福已送达'
          : '第 ' + num(stats.today) + ' 次想你已送达';
      }

      window.clearTimeout(feedbackTimer);
      feedbackTimer = window.setTimeout(clearComboFeedback, 1250);
    };

    /* 服务端回执是准的，直接盖掉本地乐观加上去的数字 */
    let bestBase = num(stats.best);
    const sync = (data) => {
      if (!data || !data.ok) return;
      if (data.dailyLimit) dailyLimit = parseInt(data.dailyLimit, 10) || dailyLimit;
      const pending = queue.length;
      Object.keys(data.totals || {}).forEach((key) => put(counters[key], data.totals[key] + (key === role ? pending : 0)));
      if (data.self) {
        selfToday = data.self.today + pending;
        const display = data.display || data.self;
        put(stats.today, display.today + pending);
        put(stats.streak, display.streak);
        put(stats.best, role === 'guest' ? Math.max(display.best, display.today + pending) : display.best);
        put(stats.total, display.total + pending);
        bestBase = display.best;
      }
      setLimitState();
    };

    const revert = (count) => {
      selfToday = Math.max(0, selfToday - count);
      put(counters[role], num(counters[role]) - count);
      put(stats.today, num(stats.today) - count);
      put(stats.total, num(stats.total) - count);
      put(stats.best, role === 'guest' ? Math.max(bestBase, num(stats.today)) : bestBase);
    };

    /**
     * 上报的是“多少毫秒之前点的”，不是客户端时间戳，
     * 访客系统时间不准也不会把奇怪的时间写进库里。
     */
    const flush = (leaving) => {
      if (!queue.length || !endpoint || (sending && !leaving)) return;

      const now = performance.now();
      const taps = queue.splice(0, 20);
      const body = new URLSearchParams();
      body.set('_', token);
      taps.forEach((at) => body.append('taps[]', String(Math.round(now - at))));

      if (leaving && navigator.sendBeacon) {
        navigator.sendBeacon(endpoint, new Blob([body.toString()], { type: 'application/x-www-form-urlencoded' }));
        return;
      }

      sending = true;
      fetch(endpoint, { method: 'POST', body: body, credentials: 'same-origin' })
        .then((response) => response.json().then((data) => (response.ok ? data : Promise.reject(data))))
        .then((data) => {
          sync(data);
          if (data.limited && isAtLimit()) setLimitState();
        })
        .catch((data) => {
          revert(taps.length);
          setLimitState();
          if (warned || !window.ccShowModal) return;
          warned = true;
          window.ccShowModal({
            title: '这一下没记上',
            // 只认接口自己回的提示，解析失败时 data 是个 JS 错误，不能拿去给人看
            message: (data && data.ok === false && data.message) || '网络似乎开了个小差，稍后再点点看。',
            eyebrow: 'HEARTBEAT',
            icon: 'heart-crack'
          });
        })
        .finally(() => {
          sending = false;
          if (queue.length) schedule();
        });
    };

    const schedule = () => {
      if (timer) return;
      timer = window.setTimeout(() => { timer = 0; flush(false); }, 500);
    };

    const stopLongPress = (keepClickSuppressed) => {
      window.clearTimeout(holdDelay);
      window.clearTimeout(holdRepeat);
      holdDelay = 0;
      holdRepeat = 0;
      holdActive = false;
      mark.classList.remove('is-holding');
      if (!keepClickSuppressed) suppressClick = false;
    };

    const holdRepeatDelay = () => {
      const progress = Math.min(Math.max(comboCount - 1, 0), 24) / 24;
      return Math.round(230 - 158 * (1 - Math.pow(1 - progress, 1.65)));
    };

    const repeatLongPress = () => {
      if (!holdActive || isAtLimit()) return;
      holdRepeat = window.setTimeout(() => {
        if (!holdActive || isAtLimit()) return;
        registerTap('hold');
        repeatLongPress();
      }, holdRepeatDelay());
    };

    const registerTap = (source) => {
      if (setLimitState()) return;
      const now = performance.now();
      comboCount = now - comboAt < 900 ? comboCount + 1 : 1;
      comboAt = now;
      applyComboPace();
      queue.push(now);
      selfToday++;

      // 先在本地加上，等回执再校准，连点时不会一顿一顿的
      put(counters[role], num(counters[role]) + 1);
      bump(counters[role]);
      put(stats.today, num(stats.today) + 1);
      bump(stats.today);
      put(stats.total, num(stats.total) + 1);
      if (role !== 'guest' && num(stats.streak) === 0) put(stats.streak, 1);
      if (role === 'guest' && num(stats.today) > num(stats.best)) put(stats.best, num(stats.today));

      beat();
      showCombo(source);
      if (selfToday === dailyLimit) {
        stopLongPress(false);
        celebrate99();
      }
      if (setLimitState()) stopLongPress(false);
      schedule();
    };

    heart.addEventListener('click', (event) => {
      if (suppressClick) {
        suppressClick = false;
        event.preventDefault();
        return;
      }
      if (isAtLimit()) {
        showLimitFeedback();
        return;
      }
      registerTap('click');
    });

    heart.addEventListener('pointerdown', (event) => {
      if (event.button !== 0 || isAtLimit()) return;
      stopLongPress(false);
      try { heart.setPointerCapture(event.pointerId); } catch (exception) { /* 某些旧浏览器不支持，仍可正常点击 */ }
      holdDelay = window.setTimeout(() => {
        holdActive = true;
        suppressClick = true;
        mark.classList.add('is-holding');
        registerTap('hold');
        if (!isAtLimit()) repeatLongPress();
      }, 430);
    });

    heart.addEventListener('pointerup', () => stopLongPress(holdActive));
    heart.addEventListener('pointercancel', () => stopLongPress(false));
    heart.addEventListener('contextmenu', (event) => event.preventDefault());

    setLimitState();

    // 关页面前把还没发出去的点击补上
    window.addEventListener('pagehide', () => flush(true));
    document.addEventListener('visibilitychange', () => {
      if (document.visibilityState === 'hidden') flush(true);
    });
  }
}());
