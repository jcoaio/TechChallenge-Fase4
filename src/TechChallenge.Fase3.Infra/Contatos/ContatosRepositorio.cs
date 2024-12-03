using System.Text;
using Dapper;
using TechChallenge.Fase3.DataTransfer.Utils;
using TechChallenge.Fase3.Domain.Contatos.Entidades;
using TechChallenge.Fase3.Domain.Contatos.Repositorios;
using TechChallenge.Fase3.Domain.Contatos.Repositorios.Filtros;
using TechChallenge.Fase3.Domain.Regioes.Entidades;
using TechChallenge.Fase3.Infra.Utils;
using TechChallenge.Fase3.Infra.Utils.DBContext;

namespace TechChallenge.Fase3.Infra.Contatos
{
    public class ContatosRepositorio(DapperContext dapperContext) : RepositorioDapper<Contato>(dapperContext), IContatosRepositorio
    {
        public async Task<PaginacaoConsulta<Contato>> ListarPaginacaoContatosAsync(ContatosPaginadosFiltro filtro)
        {
            StringBuilder sql = new(
                @"SELECT  c.id,
                          c.nome,
                          c.email,
                          c.ddd,
                          c.telefone,
                          r.ddd as RegiaoDDD,
                          r.estado,
                          r.regiao as Descricao 
                  FROM techchallenge.contatos c
                  LEFT JOIN techchallenge.regioes r
                          ON r.ddd = c.ddd
                  WHERE 1 = 1");

            if (!string.IsNullOrEmpty(filtro.Email))
                sql.AppendLine($" AND c.email = '{filtro.Email}' ");

            if (!string.IsNullOrEmpty(filtro.Nome))
                sql.AppendLine($" AND c.nome like '%{filtro.Nome}%' ");

            if (filtro.DDD > 0)
                sql.AppendLine($" AND c.ddd = {filtro.DDD} ");

            if (!string.IsNullOrEmpty(filtro.Telefone))
                sql.AppendLine($" AND c.telefone = '{filtro.Telefone}' ");

            if (!string.IsNullOrEmpty(filtro.Regiao))
                sql.AppendLine($" AND r.regiao like '%{filtro.Regiao}%' ");

            string sqlPaginado = GerarQueryPaginacao(sql.ToString(), filtro.Pg, filtro.Qt, filtro.CpOrd, filtro.TpOrd.ToString());


            Dictionary<int, Contato> registros = [];

            IEnumerable<Contato> queryResult = await session.QueryAsync<Contato, Regiao, Contato>(sqlPaginado, (contato, regiao) =>
            {
                if (!registros.TryGetValue(contato.Id.Value, out Contato? existingContato))
                {
                    existingContato = contato;
                    registros[contato.Id.Value] = existingContato;
                }
                existingContato.SetRegiao(regiao);
                return existingContato;
            }, splitOn: "RegiaoDDD");

            PaginacaoConsulta<Contato> response = new()
            {
                Registros = registros.Values,
                Total = RecuperarTotalLinhas(sql.ToString())
            };

            return response;
        }

        public async Task<List<Contato>> ListarContatosAsync(ContatoFiltro filtro)
        {
            StringBuilder sql = new(@"
                        SELECT  c.id,
                                c.nome,
                                c.email,
                                c.ddd,
                                c.telefone,
                                r.ddd as RegiaoDDD,
                                r.estado,
                                r.regiao as Descricao 
                        FROM techchallenge.contatos c
                        LEFT JOIN techchallenge.regioes r
                                ON r.ddd = c.ddd

                        WHERE 1 = 1");

            if (!string.IsNullOrEmpty(filtro.Email))
                sql.AppendLine($" AND c.email = '{filtro.Email}' ");

            if (!string.IsNullOrEmpty(filtro.Nome))
                sql.AppendLine($" AND c.nome like '%{filtro.Nome}%' ");

            if (filtro.DDD > 0)
                sql.AppendLine($" AND c.ddd = {filtro.DDD} ");

            if (!string.IsNullOrEmpty(filtro.Telefone))
                sql.AppendLine($" AND c.telefone = '{filtro.Telefone}' ");

            if (!string.IsNullOrEmpty(filtro.Regiao))
                sql.AppendLine($" AND r.regiao like '%{filtro.Regiao}%' ");

            IEnumerable<Contato> result = await session.QueryAsync<Contato, Regiao, Contato>(sql.ToString(), (contato, regiao) =>
            {
                contato.SetRegiao(regiao);
                return contato;
            }, splitOn: "RegiaoDDD");

            return result.ToList();
        }


        public async Task<Contato> InserirContatoAsync(Contato contato, CancellationToken cancellationToken)
        {
            StringBuilder sql = new(@"
                            INSERT INTO techchallenge.contatos
                                   (nome, email, ddd, telefone)
                            VALUES(@NOME, @EMAIL, @DDD, @TELEFONE);
                            SELECT c.id,
                                   r.ddd,
                                   r.estado,
                                   r.regiao AS Descricao
                              FROM techchallenge.contatos c
                              JOIN techchallenge.regioes r ON c.ddd = r.ddd
                             WHERE c.id = LAST_INSERT_ID();");

            DynamicParameters parametros = new();
            parametros.Add("@NOME", contato.Nome);
            parametros.Add("@EMAIL", contato.Email);
            parametros.Add("@DDD", contato.DDD);
            parametros.Add("@TELEFONE", contato.Telefone);

            try
            {
                dynamic result = await session.QuerySingleAsync<dynamic>(sql.ToString(), parametros);
                Regiao regiao = new(result.ddd, result.estado, result.Descricao);
                contato.SetId(result.id);
                contato.SetRegiao(regiao);
                return contato;
            }
            catch (Exception e)
            {

                throw;
            }


        }

        public async Task<Contato> RecuperarContatoAsync(int id)
        {
            StringBuilder sql = new($@"
                        SELECT  c.id,
                                c.nome,
                                c.email,
                                c.ddd,
                                c.telefone,
                                r.ddd as RegiaoDDD,
                                r.estado,
                                r.regiao as Descricao 
                        FROM techchallenge.contatos c
                        LEFT JOIN techchallenge.regioes r
                                ON r.ddd = c.ddd
                        WHERE c.id = {id}");
            return await session.QueryFirstOrDefaultAsync<Contato>(sql.ToString());
        }

        public async Task RemoverContatoAsync(int id, CancellationToken cancellationToken)
        {
            StringBuilder sql = new($@"DELETE FROM techchallenge.contatos WHERE id= {id};");

            await session.ExecuteAsync(sql.ToString());
        }

        public async Task<Contato> AtualizarContatoAsync(Contato contato, CancellationToken cancellationToken)
        {
            StringBuilder sql = new($@"
                        UPDATE techchallenge.contatos
                        SET nome='{contato.Nome}', 
                            email='{contato.Email}', 
                            ddd={contato.DDD}, 
                            telefone='{contato.Telefone}'
                        WHERE id= {contato.Id};");
            await session.ExecuteAsync(new CommandDefinition(sql.ToString(), cancellationToken: cancellationToken));
            return await RecuperarContatoAsync((int)contato.Id!);
        }
    }
}
