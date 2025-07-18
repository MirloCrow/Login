﻿using System;
using Microsoft.Data.SqlClient;

namespace FERCO.Data
{
    public static class DAOHelper
    {
        public static SqlConnection AbrirConexionSegura()
        {
            try
            {
                var conn = ConexionBD.ObtenerConexion();
                conn.Open();
                return conn;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][DAOHelper.AbrirConexionSegura] {ex.Message}");
                throw;
            }
        }

        public static int EjecutarEscalar(SqlCommand cmd)
        {
            try
            {
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][DAOHelper.EjecutarEscalar] {ex.Message}");
                return 0;
            }
        }

        public static bool EjecutarNoQuery(SqlCommand cmd)
        {
            try
            {
                return cmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][DAOHelper.EjecutarNoQuery] {ex.Message}");
                return false;
            }
        }
    }
}
