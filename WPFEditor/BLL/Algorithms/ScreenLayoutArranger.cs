using MegaMan.Common;
using MegaMan.Common.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Editor.Bll.Algorithms
{
    public class ScreenLayoutArranger
    {
        private StageDocument _stage;

        private HashSet<String> _remainingScreenNames = new HashSet<string>();

        private List<ConnectedScreenSegment> _segments = new List<ConnectedScreenSegment>();

        public ScreenLayoutArranger(StageDocument stage)
        {
            _stage = stage;
        }

        public IDictionary<String, Point> ScreenPositions
        {
            get
            {
                return _segments.SelectMany(seg => seg.Screens
                    .Select(screen => new KeyValuePair<String, Point>(
                                        screen.Screen.Name, new Point(screen.Location.X + seg.Location.X, screen.Location.Y + seg.Location.Y)
                                      )
                    ))
                    .ToDictionary(kv => kv.Key, kv => kv.Value);
            }
        }

        public void Arrange()
        {
            BuildSegments();

            var sortedSegments = _segments.OrderByDescending(s => s.Area);

            foreach (var segment in _segments)
            {
                AttemptToPlaceSegment(segment, Point.Empty);
            }
        }

        private void BuildSegments()
        {
            _remainingScreenNames.Clear();
            _segments.Clear();

            foreach (var screen in _stage.Screens)
            {
                _remainingScreenNames.Add(screen.Name);
            }

            while (_remainingScreenNames.Any())
            {
                var segment = new ConnectedScreenSegment();

                var nextScreen = _stage.Screens.Single(s => s.Name == _remainingScreenNames.First());

                segment.GrowLayout(nextScreen, _stage.Screens);

                _remainingScreenNames.RemoveWhere(s => segment.ScreenNames.Contains(s));

                _segments.Add(segment);
            }
        }

        private void AttemptToPlaceSegment(ConnectedScreenSegment segment, Point attemptedLocation)
        {
            // the attempts at placement will go in a diagonal like this diagram:
            //
            //  1 2 4 7
            //  3 5 8
            //  6 9
            //  10

            segment.Location = attemptedLocation;

            if (_segments.Where(s => s.Placed).Any(s => s.CollidesWidth(segment)))
            {
                if (attemptedLocation.X == 0)
                {
                    AttemptToPlaceSegment(segment, new Point(attemptedLocation.Y + 1, 0));
                }
                else
                {
                    AttemptToPlaceSegment(segment, new Point(attemptedLocation.X - 1, attemptedLocation.Y + 1));
                }
            }
            else
            {
                segment.Placed = true;
            }
        }
    }
}
