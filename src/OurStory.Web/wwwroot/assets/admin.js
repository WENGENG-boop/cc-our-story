(function () {
  'use strict';

  /* 配色切换：和前台共用 localStorage 里的那一项，两边保持一致
     手机顶栏和 PC 顶栏各有一个开关，两个都要接上 */
  const root = document.documentElement;
  document.querySelectorAll('[data-theme-toggle]').forEach((toggle) => {
    toggle.addEventListener('click', () => {
      const next = root.dataset.theme === 'dark' ? 'light' : 'dark';
      root.dataset.theme = next;
      try { localStorage.setItem('cc-color-mode', next); } catch (e) {}
    });
  });

  /* 手机底栏的「更多」：把不常用的入口收进一张从底部推上来的卡片 */
  const sheet = document.querySelector('[data-sheet]');
  if (sheet) {
    const openers = document.querySelectorAll('[data-sheet-open]');

    const setSheet = (open) => {
      sheet.classList.toggle('is-open', open);
      document.body.classList.toggle('sheet-open', open);
      openers.forEach((button) => button.setAttribute('aria-expanded', String(open)));
    };

    openers.forEach((button) => button.addEventListener('click', () => setSheet(!sheet.classList.contains('is-open'))));
    sheet.querySelectorAll('[data-sheet-close]').forEach((button) => button.addEventListener('click', () => setSheet(false)));

    document.addEventListener('keydown', (event) => {
      if (event.key === 'Escape' && sheet.classList.contains('is-open')) setSheet(false);
    });
  }

  /* 删除类操作二次确认，误点一下不至于直接没了 */
  document.querySelectorAll('form[data-confirm]').forEach((form) => {
    form.addEventListener('submit', (event) => {
      if (!window.confirm(form.dataset.confirm)) event.preventDefault();
    });
  });

  /* 图片库里的「复制链接」：文件名那一行放不下完整地址，靠这个按钮取 */
  document.querySelectorAll('[data-copy]').forEach((button) => {
    const label = button.querySelector('span') || button;
    const original = label.textContent;

    button.addEventListener('click', async () => {
      const text = button.dataset.copy;
      let ok = true;

      // 局域网里常常是 http，没有 clipboard API，退回到老办法
      if (navigator.clipboard && window.isSecureContext) {
        try { await navigator.clipboard.writeText(text); } catch (e) { ok = false; }
      } else {
        const holder = document.createElement('textarea');
        holder.value = text;
        holder.setAttribute('readonly', '');
        holder.style.position = 'fixed';
        holder.style.opacity = '0';
        document.body.appendChild(holder);
        holder.select();
        try { ok = document.execCommand('copy'); } catch (e) { ok = false; }
        document.body.removeChild(holder);
      }

      label.textContent = ok ? '已复制' : '复制不了，手动选一下';
      setTimeout(() => { label.textContent = original; }, 1600);
    });
  });

  /* 编辑器里的插图：上传完直接把 Markdown 图片语法插到光标处 */
  const uploadInput = document.querySelector('[data-upload-input]');
  const editor = document.querySelector('[data-editor]');
  const status = document.querySelector('[data-upload-status]');

  if (uploadInput && editor) {
    uploadInput.addEventListener('change', async () => {
      const file = uploadInput.files && uploadInput.files[0];
      if (!file) return;

      const body = new FormData();
      body.append('file', file);
      const token = document.querySelector('input[name="__RequestVerificationToken"]');
      if (token) body.append('__RequestVerificationToken', token.value);

      if (status) status.textContent = '正在上传…';

      try {
        const response = await fetch('/admin/media?handler=Upload', { method: 'POST', body: body });
        const data = await response.json();

        if (!response.ok || !data.ok) {
          if (status) status.textContent = data.error || '上传失败。';
          return;
        }

        const snippet = '\n![](' + data.url + ')\n';
        const at = editor.selectionStart || editor.value.length;
        editor.value = editor.value.slice(0, at) + snippet + editor.value.slice(at);
        editor.focus();
        editor.selectionStart = editor.selectionEnd = at + snippet.length;
        if (status) status.textContent = '已插入正文。';
      } catch (error) {
        if (status) status.textContent = '网络开了个小差，稍后再试。';
      } finally {
        uploadInput.value = '';
      }
    });
  }
}());
