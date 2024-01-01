import editorImg from '../assets/images/screens/editor.png'
import screen1 from '../assets/images/screens/1.png'
import screen6 from '../assets/images/screens/6.png'

export const Features = () => (
  <>
  <h1>Engine Features</h1>

  <div className="row section">
    <div className="col-md-6">
      <h2>Classic Mega Man Physics</h2>
      <p>The engine includes a script for Mega Man that recreates the original running speed, jump height, sliding, and shooting behaviors from the classic games. Numeric values have been taken from the original ROM files where possible.</p>
      <p>Like everything else, the script is customizable, so you can change Mega Man's behavior to your heart's content.</p>
    </div>
    <div className="col-md-6">
      <img src={screen6} />
    </div>
  </div>

  <div className="row section alt">
    <div className="col-md-6">
      <h2>Full Featured Editor</h2>
      <p>You shouldn't need to know how to code to make a fan game. Use the editor to design your tiles, map your levels, and create custom enemies, including their AI.</p>
      <p>The editor is still in development, so many features already supported by the engine are not yet in the editor.</p>
    </div>
    <div className="col-md-6">
      <img src={editorImg} />
    </div>
  </div>

  <div className="row section">
    <div className="col-md-6">
      <h2>Special Tile Types</h2>
      <p>By default, the editor gives you access to many of the environmental effects seen in the classic series, such as converyor bets, water, ice physics, quicksand, and of course, death spikes.</p>
      <p>You can customize these included types, or create your own!</p>
    </div>
    <div className="col-md-6">
      <img src={screen1} />
    </div>
  </div>

  <div className="row section alt">
    <div className="col-md-6">
      <h2>Weapon System</h2>
      <p>The weapon system allows you to quickly setup which of your custom entities can be fired as weapons. Create damage tables by setting weapon damage values on your enemies.</p>
    </div>
    <div className="col-md-6">
      <img src={screen1} />
    </div>
  </div>

  <div className="row section">
    <div className="col-md-6">
      <h2>NSF Sound Support</h2>
      <p>We recommend you add sound and music in the NSF format. Write your chiptunes in any compatible program and export to NSF.</p>
      <p>WAV is also supported, but greatly increases file sizes.</p>
    </div>
    <div className="col-md-6">
      <img src={screen1} />
    </div>
  </div>

  <div className="row section alt">
    <div className="col-md-6">
      <h2>Menus and Cutscenes</h2>
      <p>Add cutscenes with images and text, including easy-to-use text typing effects. Use the flexible menu system to create your start screen, options, and even the stage select! You can also use cutscene text to add speech bubbles!</p>
      <p className="note">Not yet supported in the editor.</p>
    </div>
    <div className="col-md-6">
      <img src={screen1} />
    </div>
  </div>

  <div className="row section">
    <div className="col-md-6">
      <h2>Disappearing Blocks</h2>
      <p>No Mega Man game is complete without these! Also known as Yoku Blocks, this staple feature is specifically supported to allow you to control the block used, the placements, and the timings of all blocks in a pattern.</p>
      <p className="note">Not yet supported in the editor.</p>
    </div>
    <div className="col-md-6">
      <img src={screen1} />
    </div>
  </div>

  <div className="row section alt">
    <div className="col-md-6">
      <h2>Much More</h2>
      <p>The engine will attempt to recreate every feature seen in the classic series. In addition to the above, it currently supports:</p>
      <ul>
        <li>Keybinding that persists across all games</li>
        <li>Easy health bar control and placement</li>
        <li>Palette control for easy color changes</li>
        <li>Autoscrolling segments</li>
        <li>Customizable bitmap fonts</li>
      </ul>
    </div>
    <div className="col-md-6">
      <img src={screen1} />
    </div>
  </div>
  </>
);