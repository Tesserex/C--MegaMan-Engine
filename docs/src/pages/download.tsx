export const Download = () => (
  <div className="row">
    <div className="col-md-12">
      <h1>Downloads</h1>

      <p>The project is still incomplete and therefore in alpha. Development snapshots will be available here as new features are completed.</p>

      <p>Report bugs <a target="_blank" href="https://github.com/Tesserex/C--MegaMan-Engine/issues">here.</a></p>

      <div className="row">
        <div className="col-md-8">
          <ul className="releases">
            <li>
              <span className="release-date pull-right">Apr 3, 2016</span>
              <h4><a href="downloads/CME-0.1.2.zip" analytics-on="click" analytics-event="Download" analytics-category="Releases" analytics-label="0.1.2">0.1.2</a></h4>
              <ul className="release-notes">
                <li>New tile editing UI with tiles moved to side panel.</li>
                <li>Tileset importer - rip tiles automatically from selected images.</li>
                <li>Can now select multiple tiles in the tileset editor for assigning properties.</li>
                <li>Option to show screen boundaries and tile properties overlay in editing window.</li>
                <li>Added snapping abilitiy for placing entities.</li>
                <li>Fixed bugs with zoom blur and cursor alignment.</li>
                <li>Added menu item to test stage from an exact selected location.</li>
                <li>Editor now has a window icon.</li>
                <li>Large amount of backend refactoring in the engine for loading projects.</li>
              </ul>
            </li>
            <li>
              <span className="release-date pull-right">Oct 28, 2015</span>
              <h4><a href="downloads/CME-0.1.1.zip" analytics-on="click" analytics-event="Download" analytics-category="Releases" analytics-label="0.1.1">0.1.1</a></h4>
              <ul className="release-notes">
                <li>Fixed the "Open" menu option.</li>
                <li>New projects include all necessary files to allow them to be played once a stage is created.</li>
                <li>Make button appearance more consistent.</li>
                <li>Fix the appearance of multiple-tile brushes when zoomed.</li>
                <li>Adding or deleting tiles from a tileset will now show those changes in the stage editor immediately.</li>
              </ul>
            </li>
            <li>
              <span className="release-date pull-right">Oct 22, 2015</span>
              <h4><a href="downloads/CME-0.1.0.zip" analytics-on="click" analytics-event="Download" analytics-category="Releases" analytics-label="0.1.0">0.1.0</a></h4>
              <ul className="release-notes">
                <li>First release to contain an early version of the editor.</li>
                <li>Expect many bugs. Many features you may expect will not be available.</li>
              </ul>
            </li>
          </ul>
        </div>
      </div>
    </div>
  </div>
);
