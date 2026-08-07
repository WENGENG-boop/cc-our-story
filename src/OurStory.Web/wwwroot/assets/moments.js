(function () {
  'use strict';

/* 图片查看器 */
  const lightbox = document.getElementById('lightbox');
  if (lightbox) {
    const image = lightbox.querySelector('img');
    const closeLightbox = () => {
      lightbox.hidden = true;
      document.body.classList.remove('modal-open');
      image.removeAttribute('src');
    };
    document.querySelectorAll('.article-body img').forEach((img) => {
      img.classList.add('is-zoomable');
      img.addEventListener('click', () => {
        image.src = img.currentSrc || img.src;
        image.alt = img.alt || '';
        lightbox.hidden = false;
        document.body.classList.add('modal-open');
        lightbox.querySelector('.lightbox-close').focus();
      });
    });
    lightbox.addEventListener('click', closeLightbox);
    document.addEventListener('keydown', (event) => {
      if (event.key === 'Escape' && !lightbox.hidden) closeLightbox();
    });
  }

/* 受密码保护的文章：就地校验，不跳转到异常页 */
  const protectedForm = document.querySelector('.article-body form.protected');
  if (protectedForm) {
    const password = protectedForm.querySelector('input[name="protectPassword"]');
    const submit = protectedForm.querySelector('input[type="submit"]');
    if (password) password.setAttribute('placeholder', '输入这篇记录的访问密码');
    if (submit) submit.value = '解锁这篇记录';

    protectedForm.addEventListener('submit', async (event) => {
      event.preventDefault();
      if (!password || !password.value) {
        window.ccShowModal({ title: '还没有输入密码', message: '输入密码后再试一次吧。', eyebrow: 'PRIVATE MOMENT', icon: 'lock' });
        return;
      }

      if (submit) {
        submit.disabled = true;
        submit.value = '验证中…';
      }

      try {
        const response = await fetch(protectedForm.action, {
          method: 'POST',
          body: new FormData(protectedForm),
          credentials: 'same-origin'
        });

        if (response.ok) {
          window.location.reload();
          return;
        }

        const html = await response.text();
        const page = new DOMParser().parseFromString(html, 'text/html');
        const detail = page.querySelector('[data-exception-message], .container');
        window.ccShowModal({
          title: '密码不正确',
          message: detail ? detail.textContent.trim() : '请检查密码后重新输入。',
          eyebrow: 'PRIVATE MOMENT',
          icon: 'circle-alert',
          button: '重新输入'
        });
        password.focus();
      } catch (error) {
        window.ccShowModal({ title: '暂时无法验证', message: '网络似乎开了个小差，请稍后再试。', eyebrow: 'OUR STORY', icon: 'wifi-off' });
      } finally {
        if (submit) {
          submit.disabled = false;
          submit.value = '解锁这篇记录';
        }
      }
    });
  }
}());
