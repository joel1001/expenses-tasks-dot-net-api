/**
 * Añade el comportamiento expansible a los elementos que ya tenés en tu app
 * con clase "month-expense-section" y "month-header".
 *
 * - Busca todos los .month-expense-section
 * - En cada uno, el .month-header es el clicable; el resto es el contenido que se oculta/muestra
 * - Inserta una flecha en el header: ▲ cuando está abierto, ▼ cuando está cerrado
 * - Al hacer clic en el header se oculta o se muestra el contenido
 *
 * Uso en tu app:
 * 1. Si usás React: importá y llamá initMonthExpenseSections() en un useEffect cuando se monte la página de gastos.
 * 2. Si usás HTML estático: cargá este script después del DOM y llamá initMonthExpenseSections().
 */

const ARROW_UP =
  '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><polyline points="18 15 12 9 6 15"/></svg>';
const ARROW_DOWN =
  '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><polyline points="6 9 12 15 18 9"/></svg>';

function getOrCreateArrowContainer(header) {
  let container = header.querySelector('.month-header-arrow');
  if (!container) {
    container = document.createElement('span');
    container.className = 'month-header-arrow';
    container.style.display = 'inline-flex';
    container.style.alignItems = 'center';
    container.style.marginRight = '8px';
    header.insertBefore(container, header.firstChild);
  }
  return container;
}

function setArrow(header, expanded) {
  const container = getOrCreateArrowContainer(header);
  container.innerHTML = expanded ? ARROW_UP : ARROW_DOWN;
}

function initMonthExpenseSections(root = document) {
  const sections = root.querySelectorAll('.month-expense-section');
  sections.forEach((section) => {
    if (section.getAttribute('data-month-expense-initialized') === 'true') return;
    const header = section.querySelector('.month-header');
    if (!header) return;
    section.setAttribute('data-month-expense-initialized', 'true');

    let expanded = section.getAttribute('data-expanded');
    if (expanded === null) expanded = 'true';
    expanded = expanded === 'true';

    const content = [];
    let next = header.nextElementSibling;
    while (next) {
      content.push(next);
      next = next.nextElementSibling;
    }

    function updateVisibility() {
      content.forEach((el) => {
        el.style.display = expanded ? '' : 'none';
      });
      setArrow(header, expanded);
      section.setAttribute('data-expanded', expanded);
    }

    setArrow(header, expanded);
    updateVisibility();

    header.style.cursor = 'pointer';
    header.setAttribute('role', 'button');
    header.setAttribute('tabindex', '0');
    header.setAttribute('aria-expanded', expanded);
    header.setAttribute('title', expanded ? 'Clic para ocultar' : 'Clic para mostrar');

    header.addEventListener('click', () => {
      expanded = !expanded;
      updateVisibility();
      header.setAttribute('aria-expanded', expanded);
      header.setAttribute('title', expanded ? 'Clic para ocultar' : 'Clic para mostrar');
    });

    header.addEventListener('keydown', (e) => {
      if (e.key === 'Enter' || e.key === ' ') {
        e.preventDefault();
        expanded = !expanded;
        updateVisibility();
        header.setAttribute('aria-expanded', expanded);
        header.setAttribute('title', expanded ? 'Clic para ocultar' : 'Clic para mostrar');
      }
    });
  });
}

export default initMonthExpenseSections;
export { initMonthExpenseSections };
