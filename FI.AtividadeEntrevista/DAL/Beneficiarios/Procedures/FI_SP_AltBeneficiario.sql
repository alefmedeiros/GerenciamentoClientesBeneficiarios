CREATE PROC FI_SP_AltBeneficiario
    @CPF VARCHAR(14),
    @Nome VARCHAR(100),
    @IdCliente BIGINT,
    @ID BIGINT
AS
BEGIN
    UPDATE BENEFICIARIOS 
    SET 
        CPF = @CPF,
        NOME = @Nome,
        IDCLIENTE = @IdCliente
    WHERE Id = @ID
END