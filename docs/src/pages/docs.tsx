import editorImg from '../assets/images/screens/editor.png'
import screen2 from '../assets/images/screens/2.png'

export const Docs = () => (
  <>
    <h1>Documentation</h1>

    <div className="row">
      <div className="col-md-6">
        <a href="#docs/editor" className="link-box">
          Editor
          <img src={editorImg} />
        </a>
      </div>

      <div className="col-md-6">
        <a href="#docs/scripting" className="link-box">
          Scripting
          <img src={screen2} />
        </a>
      </div>
    </div>

    <div className="row">
      <div className="col-md-12">
        <a href="#docs/scripting" className="link-box">
          Code
          <img src={screen2} />
        </a>
      </div>
    </div>
  </>
);
