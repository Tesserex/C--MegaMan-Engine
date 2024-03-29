class ScriptingMenu extends HTMLElement {
  connectedCallback() {
    this.innerHTML = `
      <ul class="nav nav-stacked nav-mega">
        <li><a href="/docs/scripting/sprite" data-toggle="tab" class="active">Sprite</a></li>
        <li class="active"><a data-target="#b" data-toggle="tab">Two</a></li>
        <li><a data-target="#c" data-toggle="tab">Twee</a></li>
      </ul>
    `;
  }
}

customElements.define('scripting-menu', ScriptingMenu);