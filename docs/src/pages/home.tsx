import screen1 from '../assets/images/screens/1.png'
import screen2 from '../assets/images/screens/2.png'
import screen3 from '../assets/images/screens/3.png'
import screen4 from '../assets/images/screens/4.png'
import screen5 from '../assets/images/screens/5.png'

export const Home = () => (
  <>
  <div className="row section">
    <div className="col-md-12">
      <p>The C# Mega Man Engine was built from the ground up to allow Mega Man fans to craft high-quality fan games with ease. The goal of the project is to faithfully recreate all of the feel and features of the classic NES series. The project has come a long way, and is still under active development.</p>
    </div>
  </div>

  <div className="row section alt">
    <div className="col-md-6">
      <p>Recreate your favorite or most feared stages from the classic series. Customize your tiles, enemies, and more.</p>
    </div>
    <div className="col-md-6">
      <img src={screen1} />
    </div>
  </div>

  <div className="row section">
    <div className="col-md-6">
      <img src={screen2} />
    </div>
    <div className="col-md-6">
      <p>Easily add special stage gimmicks, like conveyors, ice physics, death spikes, and water sections.</p>
    </div>
  </div>

  <div className="row section alt">
    <div className="col-md-6">
      <p>Create your own cutscenes and menus, with animation, text, and entities. You have total control over the results!</p>
      <p>By the way, this is not a clip of Mega Man 5 - this scene was completely recreated in the engine!</p>
    </div>
    <div className="col-md-6">
      <img src={screen3} />
    </div>
  </div>

  <div className="row section">
    <div className="col-md-6">
      <img src={screen4} />
    </div>
    <div className="col-md-6">
      <p>Of course, what would Mega Man be without special weapons? Create your own new arsenal, with custom movement patterns, damage, and animations.</p>
    </div>
  </div>

  <div className="row section alt">
    <div className="col-md-6">
      <p>The engine can even simulate the appearance of an old NTSC television, with scaling and scanlines! Choose from Composite, S-Video, or RGB connections!</p>
    </div>
    <div className="col-md-6">
      <img src={screen5} />
    </div>
  </div>
  </>
);
