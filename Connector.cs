using System.Windows;
namespace WPFSamples.Samples.HousePlan
{
    public class Connector: DiagramObject
    {
        public override BindablePoint Location
        {
            get { return new BindablePoint(); }
        }
    	  
        private SnapSpot _start;
        public SnapSpot Start
        {
            get { return _start; }
            set
            {
                _start = value;
                OnPropertyChanged("Start");
            }
        }

        private SnapSpot _end;
        public SnapSpot End
        {
            get { return _end; }
            set
            {
                _end = value;
                OnPropertyChanged("End");
                MidPoint.Value = new Point(((End.Location.X + Start.Location.X) / 2), 
                                     ((End.Location.Y + Start.Location.Y) / 2));
            }
        }

        private BindablePoint _midPoint;
        public BindablePoint MidPoint
        {
            get { return _midPoint ?? (_midPoint = new BindablePoint()); }
        }
    }
}