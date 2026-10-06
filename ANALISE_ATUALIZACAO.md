# Análise e Atualização do Programa - Resumo

## Análise Realizada

Este documento resume a análise do programa MCDSaveEdit (Editor de Arquivos de Save do Minecraft: Dungeons) e as atualizações implementadas.

## Resumo Executivo

O programa foi analisado e identificou-se várias oportunidades de atualização, principalmente relacionadas a **vulnerabilidades de segurança** em pacotes NuGet desatualizados.

## Vulnerabilidades Encontradas

### 1. **System.Text.Json 6.0.6** - CRÍTICO ⚠️
- **CVE**: CVE-2024-43485
- **Severidade**: Alta
- **Problema**: Vulnerabilidade de negação de serviço (DoS) ao desserializar entrada em modelos usando a propriedade `[ExtensionData]`
- **Solução Aplicada**: Atualizado para versão 6.0.10
- **Projetos Atualizados**: MCDSaveEdit e DungeonTools

### 2. **System.Net.Http 4.3.4** - IMPORTANTE ⚠️
- **Status**: Fim da Vida Útil (EOL - End of Life)
- **Problema**: Microsoft não fornece mais patches de segurança para esta versão
- **Solução**: Não foi possível atualizar nesta mudança (requer migração para .NET 6+)
- **Mitigação**: O .NET Framework 4.8 inclui System.Net.Http no framework

### 3. **Newtonsoft.Json 13.0.1** - OK ✅
- **Status**: Já estava em versão segura
- **CVE Evitado**: CVE-2024-21907 (afeta versões < 13.0.1)

## Atualizações Implementadas

### Pacotes Atualizados

1. **System.Text.Json**: 6.0.6 → 6.0.10
   - Projetos: MCDSaveEdit, DungeonTools
   - Razão: Correção de vulnerabilidade crítica CVE-2024-43485

2. **NLog**: 5.0.4 → 5.3.4
   - Projeto: MCDSaveEdit
   - Razão: Correções de bugs e melhorias (sem vulnerabilidades conhecidas)

3. **System.Diagnostics.DiagnosticSource**: 6.0.0 → 6.0.1
   - Projeto: MCDSaveEdit
   - Razão: Atualização menor para correções de bugs

### Arquivos Modificados

- `MCDSaveEdit/packages.config`
- `MCDSaveEdit/MCDSaveEdit.csproj`
- `DungeonTools/packages.config`
- `DungeonTools/DungeonTools.csproj`
- `SECURITY_UPDATES.md` (novo arquivo com documentação detalhada)

## Como Aplicar as Atualizações

### Para Desenvolvedores

1. Clone ou atualize o repositório com as mudanças
2. Abra a solução no Visual Studio 2022
3. Clique com botão direito na solução e selecione "Restore NuGet Packages"
4. Compile a solução normalmente

O Visual Studio baixará automaticamente as versões atualizadas dos pacotes do NuGet.

## Testes Recomendados

Após aplicar as atualizações, recomenda-se testar:

1. ✅ Verificar se a aplicação compila sem erros
2. ✅ Testar carregamento e salvamento de arquivos de personagem
3. ✅ Testar todas as funcionalidades principais (edição de inventário, encantamentos, etc.)
4. ✅ Garantir que o carregamento de imagens dos arquivos .pak ainda funciona

## Recomendações Futuras

### 1. Migração para .NET 6 ou Superior
**Por quê?**
- Eliminaria a dependência EOL do System.Net.Http 4.3.4
- Melhor performance
- Suporte multiplataforma (já parcialmente implementado para Steam Deck)
- Atualizações de segurança de longo prazo

### 2. Atualizações Regulares de Dependências
- Revisar pacotes NuGet trimestralmente
- Verificar avisos de segurança do GitHub

### 3. Ferramentas de Segurança Automatizadas
- GitHub Dependabot (já disponível gratuitamente)
- Snyk
- OWASP Dependency-Check

## Estado Atual do Projeto

### ✅ Segurança Melhorada
- Vulnerabilidade crítica CVE-2024-43485 corrigida
- Pacotes atualizados para versões estáveis mais recentes

### ⚠️ Atenção Necessária
- System.Net.Http 4.3.4 permanece EOL (requer migração futura)

### 📝 Bem Documentado
- Arquivo SECURITY_UPDATES.md criado com informações detalhadas
- Histórico de alterações no Git

## Conclusão

O programa foi atualizado com sucesso para corrigir vulnerabilidades de segurança conhecidas. As mudanças são mínimas e focadas, alterando apenas referências de pacotes NuGet sem modificar código fonte. O aplicativo mantém compatibilidade com .NET Framework 4.8 e Visual Studio 2022.

**Próximos Passos Recomendados:**
1. Testar a aplicação após a atualização
2. Considerar migração para .NET 6+ no futuro
3. Configurar verificação automática de dependências

## Referências

- [CVE-2024-43485 - System.Text.Json DoS](https://github.com/dotnet/announcements/issues/329)
- [CVE-2024-21907 - Newtonsoft.Json DoS](https://nvd.nist.gov/vuln/detail/CVE-2024-21907)
- [NLog Release Notes](https://github.com/NLog/NLog/releases)
- [SECURITY_UPDATES.md](./SECURITY_UPDATES.md) - Documentação detalhada em inglês

---

# Segunda Rodada — Auditoria de Código e Correções

Esta rodada revisou o código-fonte (não apenas dependências). Cada correção abaixo está coberta por teste em `MCDSaveEditTests`; os 15 testes de comportamento novos falham no código anterior e passam no atual.

## Integridade dos saves (risco de perda de dados)

| Problema | Efeito | Correção |
|---|---|---|
| Campos do save não mapeados eram descartados ao salvar | Dados de versões novas do jogo (ou não modelados) sumiam do arquivo | `[JsonExtensionData]` em todos os modelos (`ProfileSaveFile`, `Item`, `Enchantment`, etc.) |
| O parser tratava cada byte como `char` e removia bytes 0x80–0x9F | Nomes/textos não-ASCII (ex.: `Ł`, `中文`) eram corrompidos | Só bytes de controle ASCII são removidos; UTF-8 preservado |
| Chaves `{`/`}` dentro de strings contavam como estrutura | Objeto JSON cortado no meio | Parser ignora chaves dentro de strings (com escape `\"`) |
| Gravação truncava o arquivo e escrevia por cima | Falha no meio deixava o save destruído | Gravação em arquivo temporário + substituição; versão anterior preservada como `.bak` |
| Exceções ao abrir/salvar (arquivo bloqueado pelo jogo, acesso negado) | Encerramento abrupto do app | Erros capturados e exibidos em mensagem |

## Bugs de lógica

- **XP com overflow**: `experienceForLevel` usava aritmética `int`; a partir do nível 2676 gravava XP **negativo** no save. Agora usa precisão decimal e satura em `long.MaxValue`; `levelForExperience` corrige erros de ponto flutuante.
- **Custo de encantamento** e **nível por poder** também tinham overflow em valores altos.
- **Mover item equipado para o baú**: o item mantinha `equipmentSlot`, ficava invisível na lista do baú e a tela de equipamento não atualizava.
- **Conjunto de encantamentos incompleto** (menos de 3) derrubava a tela com `IndexOutOfRangeException`; adicionar slot agora completa o conjunto.
- **Salvar encantamento fora da lista** (ex.: o encantamento dourado) gerava `ArgumentOutOfRangeException`.
- **Moedas** com `currency` ausente no save geravam `NullReferenceException`.
- **Estatísticas**: um campo presente em `progressStatCounters` e `mob_kills` era sempre gravado no primeiro.
- Itens sem `inventoryIndex` derrubavam a ordenação da lista.
- `removing()` usava `Except`, que também remove duplicatas de outros elementos.

## Robustez no carregamento

- Exceções durante a leitura dos `.pak` ou o pré-carregamento de imagens deixavam a splash **travada para sempre** (`TaskCompletionSource` nunca concluído).
- Chaves duplicadas em localizações, encantamentos e equipamentos abortavam todo o carregamento do conteúdo do jogo.
- Strings de idioma: namespaces ausentes ou chaves repetidas não descartam mais todo o conjunto.
- Verificação de versão aceita tags `v1.2.3` e valores inválidos sem lançar exceção.
- Espera pelas configs remotas deixou de ser um laço ocupado que consumia CPU e inundava o console.
- Chaves do Registro do Windows passaram a ser liberadas (`using`).

## Segurança das dependências

Uma auditoria com `dotnet list package --vulnerable --include-transitive` sobre todos os `packages.config` encontrou duas vulnerabilidades altas que a rodada anterior não cobriu:

- **SkiaSharp 2.88.3 → 2.88.9** (GHSA-j7hp-h8jx-5ppr, falha do libwebp — CVE-2023-4863). A falha está no `libSkiaSharp.dll` nativo, que o Costura embute a partir de `Costura32/` e `Costura64/`; esses binários eram idênticos (SHA-256) aos da 2.88.3 e foram substituídos pelos do pacote oficial 2.88.9, com assinaturas Microsoft e NuGet.org verificadas.
- **DotNetZip 1.16.0** (GHSA-xhg6-9j5j-w4vf): não existe versão corrigida. A falha afeta extração de arquivos zip para disco, o que este programa não faz; a substituição depende do submódulo PakReader (ver `SECURITY_UPDATES.md`).

## Como foi validado

O projeto WPF/.NET Framework não compila em Linux e os submódulos não estavam acessíveis no ambiente de análise. A validação foi feita em duas frentes:

1. **Compilação do aplicativo inteiro** (todos os `.cs` de `MCDSaveEdit`, C# 8, nullable) contra os assemblies de referência WPF, com stubs apenas para os submódulos, o `Secrets.cs` e os campos gerados pelo XAML: **sem erros**.
2. **Testes**: os arquivos de lógica do repositório (modelos, parser, cálculos, view models, utilitários) rodaram num projeto de teste .NET 8 com a suíte existente e os testes novos: **49/49 aprovados**.

A interface WPF, a criptografia (`DungeonTools`) e a leitura dos `.pak` não foram executadas — recomenda-se um teste manual de abrir, editar e salvar um personagem no Windows antes de publicar uma versão.
