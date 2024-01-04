class Nav extends HTMLElement {
  connectedCallback() {
    this.innerHTML = `
    <nav class="navbar navbar-dark navbar-expand-lg fixed-top">
    <div class="container">
      <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav" aria-controls="navbarNav" aria-expanded="false" aria-label="Toggle navigation">
        <span class="navbar-toggler-icon"></span>
      </button>
      <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav">
          <li class="nav-item"><a class="nav-link" href="/">Home</a></li>
          <li class="nav-item"><a class="nav-link" href="/download">Download</a></li>
          <li class="nav-item"><a class="nav-link" href="/features">Features</a></li>
          <li class="nav-item"><a class="nav-link" target="_blank" href="http://www.youtube.com/playlist?list=PL51A9554B5210B6F6">YouTube</a></li>
          <li class="nav-item"><a class="nav-link" href="/docs">Documentation</a></li>
          <li class="nav-item"><a class="nav-link" href="/resources">Resource Packs</a></li>
        </ul>
      </div>
    </div>
  </nav>
    `;
  }
}

customElements.define('main-nav', Nav);

class Logo extends HTMLElement {
  connectedCallback() {
    this.innerHTML = `
      <div class="row logo">
        <div class="col-md-12">
          <img src="/assets/images/logo.png" />
        </div>
      </div>
    `;
  }
}

customElements.define('main-logo', Logo);

class Footer extends HTMLElement {
  connectedCallback() {
    this.innerHTML = `
    <footer class="footer">
      <div class="container">
        <div class="d-flex justify-content-between">
          <div>C# Mega Man Engine maintained by <a href="https://github.com/Tesserex">Tesserex</a></div>
          <div>Published with <a href="http://pages.github.com">GitHub Pages</a></div>
        </div>
      </div>
    </footer>
    `;
  }
}

customElements.define('main-footer', Footer);
