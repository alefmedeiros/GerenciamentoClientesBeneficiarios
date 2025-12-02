using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FI.AtividadeEntrevista.BLL
{
    public class BoBeneficiario
    {
        /// <summary>
        /// Inclui um novo beneficiário
        /// </summary>
        /// <param name="beneficiario">Objeto de beneficiário</param>
        public long Incluir(DML.Beneficiario beneficiario)
        {
            DAL.DaoBeneficiario dao = new DAL.DaoBeneficiario();
            return dao.Incluir(beneficiario);
        }

        /// <summary>
        /// Altera um beneficiário
        /// </summary>
        /// <param name="beneficiario">Objeto de beneficiário</param>
        public void Alterar(DML.Beneficiario beneficiario)
        {
            DAL.DaoBeneficiario dao = new DAL.DaoBeneficiario();
            dao.Alterar(beneficiario);
        }

        /// <summary>
        /// Consulta o beneficiário pelo id
        /// </summary>
        /// <param name="id">id do beneficiário</param>
        public DML.Beneficiario Consultar(long id)
        {
            DAL.DaoBeneficiario dao = new DAL.DaoBeneficiario();
            return dao.Consultar(id);
        }

        /// <summary>
        /// Lista beneficiários de um cliente
        /// </summary>
        /// <param name="idCliente">id do cliente</param>
        public List<DML.Beneficiario> ListarPorCliente(long idCliente)
        {
            DAL.DaoBeneficiario dao = new DAL.DaoBeneficiario();
            return dao.ListarPorCliente(idCliente);
        }

        /// <summary>
        /// Excluir o beneficiário pelo id
        /// </summary>
        /// <param name="id">id do beneficiário</param>
        public void Excluir(long id)
        {
            DAL.DaoBeneficiario dao = new DAL.DaoBeneficiario();
            dao.Excluir(id);
        }

        /// <summary>
        /// Verifica se já existe um beneficiário com o CPF informado para o cliente
        /// </summary>
        /// <param name="CPF">CPF do beneficiário</param>
        /// <param name="IdCliente">Id do cliente</param>
        public bool VerificarExistencia(string CPF, long IdCliente)
        {
            DAL.DaoBeneficiario dao = new DAL.DaoBeneficiario();
            return dao.VerificarExistencia(CPF, IdCliente);
        }

        /// <summary>
        /// Verifica se já existe um beneficiário com o CPF informado para o cliente (excluindo o próprio)
        /// </summary>
        /// <param name="CPF">CPF do beneficiário</param>
        /// <param name="IdCliente">Id do cliente</param>
        /// <param name="Id">Id do beneficiário</param>
        public bool VerificarExistencia(string CPF, long IdCliente, long Id)
        {
            DAL.DaoBeneficiario dao = new DAL.DaoBeneficiario();
            return dao.VerificarExistencia(CPF, IdCliente, Id);
        }

        /// <summary>
        /// Valida se o CPF é válido de acordo com o cálculo do dígito verificador
        /// </summary>
        /// <param name="cpf">CPF a ser validado</param>
        public bool ValidarCPF(string cpf)
        {
            // Remove caracteres não numéricos
            cpf = new string(cpf.Where(char.IsDigit).ToArray());

            // CPF deve ter 11 dígitos
            if (cpf.Length != 11)
                return false;

            // Verifica se todos os dígitos são iguais (ex: 111.111.111-11)
            bool todosDigitosIguais = true;
            for (int i = 1; i < cpf.Length; i++)
            {
                if (cpf[i] != cpf[0])
                {
                    todosDigitosIguais = false;
                    break;
                }
            }
            if (todosDigitosIguais)
                return false;

            int soma = 0;
            for (int i = 0; i < 9; i++)
                soma += int.Parse(cpf[i].ToString()) * (10 - i);

            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;

            if (int.Parse(cpf[9].ToString()) != digito1)
                return false;

            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(cpf[i].ToString()) * (11 - i);

            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;

            return int.Parse(cpf[10].ToString()) == digito2;
        }
    }
}