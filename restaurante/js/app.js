// Pequeño puente JS para Blazor: localStorage y medidas del plano.
window.appStorage = {
    get: key => localStorage.getItem(key),
    set: (key, value) => localStorage.setItem(key, value)
};

window.appBoard = {
    rect: el => {
        const r = el.getBoundingClientRect();
        return { left: r.left, top: r.top, width: r.width, height: r.height };
    }
};
