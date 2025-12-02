using FI.AtividadeEntrevista.DML;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FI.AtividadeEntrevista.DAL
{
    /// <summary>
    /// Classe de acesso a dados de Beneficiário
    /// </summary>
    internal class DaoBeneficiario : AcessoDados
    {
        /// <summary>
        /// Inclui um novo beneficiário
        /// </summary>
        /// <param name="beneficiario">Objeto de beneficiário</param>
        internal long Incluir(DML.Beneficiario beneficiario)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", beneficiario.CPF));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Nome", beneficiario.Nome));
            parametros.Add(new System.Data.SqlClient.SqlParameter("IdCliente", beneficiario.IdCliente));

            DataSet ds = base.Consultar("FI_SP_IncBeneficiario", parametros);
            long ret = 0;
            if (ds.Tables[0].Rows.Count > 0)
                long.TryParse(ds.Tables[0].Rows[0][0].ToString(), out ret);
            return ret;
        }

        /// <summary>
        /// Altera um beneficiário
        /// </summary>
        /// <param name="beneficiario">Objeto de beneficiário</param>
        internal void Alterar(DML.Beneficiario beneficiario)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", beneficiario.CPF));
            parametros.Add(new System.Data.SqlClient.SqlParameter("Nome", beneficiario.Nome));
            parametros.Add(new System.Data.SqlClient.SqlParameter("IdCliente", beneficiario.IdCliente));
            parametros.Add(new System.Data.SqlClient.SqlParameter("ID", beneficiario.Id));

            base.Executar("FI_SP_AltBeneficiario", parametros);
        }

        /// <summary>
        /// Consulta um beneficiário pelo Id
        /// </summary>
        /// <param name="Id">Id do beneficiário</param>
        internal DML.Beneficiario Consultar(long Id)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("Id", Id));

            DataSet ds = base.Consultar("FI_SP_ConsBeneficiario", parametros);
            List<DML.Beneficiario> beneficiarios = Converter(ds);

            return beneficiarios.FirstOrDefault();
        }

        /// <summary>
        /// Lista beneficiários por cliente
        /// </summary>
        /// <param name="idCliente">Id do cliente</param>
        internal List<DML.Beneficiario> ListarPorCliente(long idCliente)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("IdCliente", idCliente));

            DataSet ds = base.Consultar("FI_SP_ListBeneficiariosPorCliente", parametros);
            List<DML.Beneficiario> beneficiarios = Converter(ds);

            return beneficiarios;
        }

        /// <summary>
        /// Excluir Beneficiário
        /// </summary>
        /// <param name="Id">Id do beneficiário</param>
        internal void Excluir(long Id)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("Id", Id));

            base.Executar("FI_SP_DelBeneficiario", parametros);
        }

        /// <summary>
        /// Verifica se já existe um beneficiário com o CPF informado para o cliente
        /// </summary>
        /// <param name="CPF">CPF do beneficiário</param>
        /// <param name="IdCliente">Id do cliente</param>
        internal bool VerificarExistencia(string CPF, long IdCliente)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", CPF));
            parametros.Add(new System.Data.SqlClient.SqlParameter("IdCliente", IdCliente));

            DataSet ds = base.Consultar("FI_SP_VerificaBeneficiario", parametros);

            return ds.Tables[0].Rows.Count > 0;
        }

        /// <summary>
        /// Verifica se já existe um beneficiário com o CPF informado para o cliente (excluindo o próprio)
        /// </summary>
        /// <param name="CPF">CPF do beneficiário</param>
        /// <param name="IdCliente">Id do cliente</param>
        /// <param name="Id">Id do beneficiário</param>
        internal bool VerificarExistencia(string CPF, long IdCliente, long Id)
        {
            List<System.Data.SqlClient.SqlParameter> parametros = new List<System.Data.SqlClient.SqlParameter>();

            parametros.Add(new System.Data.SqlClient.SqlParameter("CPF", CPF));
            parametros.Add(new System.Data.SqlClient.SqlParameter("IdCliente", IdCliente));
            parametros.Add(new System.Data.SqlClient.SqlParameter("ID", Id));

            DataSet ds = base.Consultar("FI_SP_VerificaBeneficiarioComId", parametros);

            return ds.Tables[0].Rows.Count > 0;
        }

        private List<DML.Beneficiario> Converter(DataSet ds)
        {
            List<DML.Beneficiario> lista = new List<DML.Beneficiario>();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    DML.Beneficiario beneficiario = new DML.Beneficiario();
                    beneficiario.Id = row.Field<long>("Id");
                    beneficiario.CPF = row.Field<string>("CPF");
                    beneficiario.Nome = row.Field<string>("Nome");
                    beneficiario.IdCliente = row.Field<long>("IdCliente");
                    lista.Add(beneficiario);
                }
            }

            return lista;
        }
    }
}