using static CMI.Utils;

namespace CMI.Network
{
    public sealed class Cell : LSTM
    {
        public float x { get; set; }
        public float ht_1 { get; set; }
        public float ct_1 { get; set; }
        public float ct { get; set; }
        public float ht { get; set; }
        public float a { get; set; }
        public float i { get; set; }
        public float f { get; set; }
        public float o { get; set; }

        //public float dx { get; set; }
        public float dht { get; set; }
        public float dht_1 { get; set; }
        public float dct { get; set; }
        public float da { get; set; }
        public float di { get; set; }
        public float df { get; set; }
        public float do_ { get; set; }

        public float dloss { get; set; }

        public Cell(float x, float ct_1, float ht_1) : base()
        {
            this.x = x;
            this.ct_1 = ct_1;
            this.ht_1 = ht_1;
        }

        public void Forward()
        {
            UpdateGate();
            ForgetGate();
            OutputGate();
        }

        public void Backpropagation(float target, float next_dht, float next_dct, float next_f)
        {
            dloss = ht - target;
            dht = dloss + next_dht;
            dct = dht * o * (1 - Tanh2(ct)) + next_dct * next_f;

            da = dct * i * (1 - (float)Math.Pow(a, 2));
            di = dct * a * i * (1 - i);
            df = dct * ct_1 * f * (1 - f);
            do_ = dht * Tanh(ct) * o * (1 - o);

            //dx = Wa * da + Wi * di + Wf * df + Wo * do_; // it's never used
            dht_1 = Ua * da + Ui * di + Uf * df + Uo * do_;
        }

        private void UpdateGate()
        {
            a = Tanh(Wa * x + Ua * ht_1 + ba);
            i = Sigmoid(Wi * x + Ui * ht_1 + bi);
        }

        private void ForgetGate()
        {
            f = Sigmoid(Wf * x + Uf * ht_1 + bf);
            ct = f * ct_1 + a * i;
        }

        private void OutputGate()
        {
            o = Sigmoid(Wo * x + Uo * ht_1 + bo);
            ht = Tanh(ct) * o;
        }
    }
}
