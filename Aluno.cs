namespace DIO
{
    public struct Aluno
    {
        public string Name { get; set; }
        public decimal Note { get; set; }
        public Conceito Conceito;

        public void SetConceito(Conceito a)
        {
            this.Conceito = a;
        }

        public Conceito GetConceito()
        {
            return this.Conceito;
        }
    }
}