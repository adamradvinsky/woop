namespace woop_app
{

    

    public class Row{

        public float time;
        public int hr;
        public float ax;
        public float ay;
        public float az;
        
        public byte[] bytes;

        public Row(float time, int hr, float ax, float ay, float az, byte[] bytes){
            this.time = time;
            this.hr = hr;
            this.ax = ax;
            this.ax = ay;
            this.ax = az;
            this.bytes = bytes;
        }
    }
}