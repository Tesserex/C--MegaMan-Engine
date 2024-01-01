import { Nav } from './pages/nav'
import { Footer } from './pages/footer'
import { Route, Routes } from 'react-router-dom'
import { Docs, Download, Features, Home, Resources } from './pages'
import logo from './assets/images/logo.png'

function App() {
  return (
    <>
      <Nav />
      
      <div id="main" className="container">
        <div className="row logo">
          <div className="col-md-12">
            <img src={logo} />
          </div>
        </div>

        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/download" element={<Download />} />
          <Route path="/features" element={<Features />} />
          <Route path="/docs" element={<Docs />} />
          <Route path="/resources" element={<Resources />} />
        </Routes>
      </div>

      <Footer />
    </>
  )
}

export default App
