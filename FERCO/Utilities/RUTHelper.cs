namespace FERCO.Utilities
{
    public static class RutHelper
    {
        public static string CalcularDV(int rut)
        {
            int suma = 0;
            int multiplicador = 2;

            while (rut > 0)
            {
                int digito = rut % 10;
                suma += digito * multiplicador;
                rut /= 10;
                multiplicador = multiplicador == 7 ? 2 : multiplicador + 1;
            }

            int resto = suma % 11;
            int dv = 11 - resto;

            return dv == 11 ? "0" : dv == 10 ? "K" : dv.ToString();
        }

        public static string FormatearRUT(int rut)
        {
            string dv = CalcularDV(rut);
            return $"{rut:N0}-{dv}".Replace(",", "."); // ejemplo: 12345678 -> 12.345.678-K
        }
    }
}
