/* ============================================================
   Shared ES/EN i18n for demo pages.
   Each page defines window.PAGE_I18N = { es:{...}, en:{...} }
   before loading this script. Elements are tagged with
   data-i18n (textContent) or data-i18n-html (innerHTML).
   The language choice is shared with the main site via the
   'lang' localStorage key.
============================================================ */
(function () {
  const dicts = window.PAGE_I18N || { es: {}, en: {} };

  // Build the floating ES/EN switch
  const sw = document.createElement('div');
  sw.className = 'lang-switch lang-float';
  sw.setAttribute('aria-label', 'Language selector');
  sw.innerHTML = '<button data-lang="es">ES</button><button data-lang="en">EN</button>';
  document.body.appendChild(sw);

  function apply(lang) {
    if (lang !== 'es' && lang !== 'en') lang = 'es';
    const dict = dicts[lang] || {};
    document.querySelectorAll('[data-i18n]').forEach(el => {
      const k = el.getAttribute('data-i18n');
      if (dict[k] != null) el.textContent = dict[k];
    });
    document.querySelectorAll('[data-i18n-html]').forEach(el => {
      const k = el.getAttribute('data-i18n-html');
      if (dict[k] != null) el.innerHTML = dict[k];
    });
    document.querySelectorAll('[data-i18n-ph]').forEach(el => {
      const k = el.getAttribute('data-i18n-ph');
      if (dict[k] != null) el.setAttribute('placeholder', dict[k]);
    });
    if (dict['_title'] != null) document.title = dict['_title'];
    document.documentElement.lang = lang;
    sw.querySelectorAll('button').forEach(b => b.classList.toggle('active', b.dataset.lang === lang));
    try { localStorage.setItem('lang', lang); } catch (e) {}
    document.dispatchEvent(new CustomEvent('langchange', { detail: lang }));
  }

  sw.querySelectorAll('button').forEach(b =>
    b.addEventListener('click', () => apply(b.dataset.lang)));

  let saved;
  try { saved = localStorage.getItem('lang'); } catch (e) {}
  // demo pages only offer es/en; map any other saved value (e.g. de) to en
  const browser = (navigator.language || 'es').slice(0, 2);
  const initial = (saved === 'es' || saved === 'en') ? saved
                : (saved ? 'en' : (browser === 'es' ? 'es' : 'en'));
  apply(initial);

  window.__applyDemoLang = apply;
})();
