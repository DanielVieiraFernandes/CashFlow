using System.Collections;

namespace WebApi.Test.InlineData;

public class CultureInlineDataTest : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
        // Com o yield return, podemos retornar o valor
        // e continuar a execução do método após o método
        // que chamou o enumerador terminar de ser executado.
        //-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*
        yield return new object[] { "pt-BR", };
        yield return new object[] { "en" };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
