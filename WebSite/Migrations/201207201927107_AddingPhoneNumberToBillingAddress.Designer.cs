// <auto-generated />
namespace WebSite.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    
    public sealed partial class AddingPhoneNumberToBillingAddress : IMigrationMetadata
    {
        string IMigrationMetadata.Id
        {
            get { return "201207201927107_AddingPhoneNumberToBillingAddress"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return "H4sIAAAAAAAEAO1dzXLjuBG+pyrvoNIp2aq1bM9md3bK3i2Pxk654rFdI8/s0QWTsM0aitSSlNfOq+WQR8orBOAvfhq/pCR6kptEAI1G40MDaDQa//nXv49+fV7Gkyec5VGaHE8P9vanE5wEaRglD8fTdXH//dvpr7/8+U9Hp+HyefKlyXdI85GSSX48fSyK1bvZLA8e8RLle8soyNI8vS/2gnQ5Q2E6O9zffzs72J9hQmJKaE0mR5/WSREtcfmH/J2nSYBXxRrFH9MQx3n9naQsSqqTS7TE+QoF+Hj6G75bRAXe+4AKdIdyPJ2cxBEifCxwfO/I1P7PlKlpWx2p8JQwVrzcvKxwWenx9HOOMzYHyfMP/MJ9IJ+us3SFs+LlE75nyp2H08mMLzsTC7dFhXKUhePpeVK8OZxOLtdxjO5i8uEexbTNqx/fLYo0w3/HCc5QgcNrVBQ4I11zHuKyCbUo3q1+tJPGz7P9QyqNGUqStEAF6WeJeYHVpiry+SkKiZgMTLtRO8/zNQ55cSyKjABzOjmLnnF4gZOH4rGl/hE9N1/+RtD5OYkIjEmZIltjZ2ZOlyiKT8Iww3nuWPnBfu/az6IsL+jPrbf7Au2o4vP8PQEebnv6fZrGGCXOdBbRQ/J5RZRD2wT6+yZa+sniIn2IkkGo3WQRik+fV1H2Mgi9xfouD7JoRQeqRl9UnaEndZkW0X0UlGN+gYuC9HhuVkECzUv0FD2UJDSMTiefcFxmyh+jVaWz9+gQv+VznWXp8lMa1+qQS7xdpOssoPJLVTluUPaACz8OczsWcy2PuZnJ3JlLSgjmjqbc0ho4rrqvbVUNN0xSw6gtFxBW1CKDcwuSgzLBAgRzQnI8mnXzuHZ252DnMcuLo9B1trccxWOa9VmWKbvOmkKgd7IuHtMs+ie+PL2BxbG9ifckKKKnUgqD6Og5IuvaOLYiCCpqK5VFaYJDkFebUoluGGozSkNRn3uw4Vgy2XNINvjsMyzt"
                       + "MD6KoXmdRUEHMhxESxRPJ9cZ+VXv795OJ4sAUb7dByorlLMM/74mG8WXvqP/PD95IkttWuImrecwxxWg1SBpGTaOlArzcFF4yKhLaMeOpthgg6jjvedI4jq8z3ByQM4oxpTHbujQcR6y7lmKIZ+OpOV8+q0p9/9uMqmaWnP5LM5FFQGs273AAq69PcAD0PHBkoLMK4DWJxzg6Al/INPUy0lMEtynKJjiVakQr6Pg61AUibCCr0MS/A3jrwO3uurL+ITIf02Wx0vSj46k7WdCKo4/ooRUmH/Eyzs/S25V0gfzXclXAPP/QaNjHytv/9qvUZ7/kWY72Oae5xf4AQUv9ZjYzOCbZziMijkiDfQYdF1pn4HHl34Fg4+y+pjGIc48BsIAiLhcs0iwrJf87D0CS4N4f0PLly+OvP+g4dzGZl5rjeHM5O+jOCaMt9oIWEp2qL4Vc3crS2UmaaGpztlr3dnw5DHoGaG6jnjr/hjFcK+5vYgSfLADK2dX++H2a5+XInaq9af+cy09VCarzatsUTCGWMv63/SvP80LFM8Jgc1vUsWqH9MEe+n3/nXPyRK/yNyNhEotWRME1WOjvdo8nVIUkiRVKKb3UoBN/T6rnk5ezkseW1GPQgFuYbmv6q+TPE+DqORSYe1XWH/5Bp0m4cTPFFw1GjI7k/au4yJaxVFAGD+efieJ0bnW9ugXrpWxbfNVH0xFtF4lH3CMCzyhh2T0MGGO8gCF8uAlcg/5LwTgOKPYoiowyYsMRUkhjwaioKMVir0aKFDrY8+mTWiZEVM+4BVO6Djx6vyBuWyZETrAJO+jGTMI7MeGfIZoA07NgSKMSd9RoD6LdBhy48K9skmuQIIPQr2RruzT4fjaArZlxyMVzDReSB20Sk9RB+yqHZcsB8b+3t6BRN8Lc0pWXPuzJ8aUch6Gj11gStwIa/o/t0UV2PFawo7I+k5cSlm3nznuUrEHnX11bJVnnA6jCHJzcxqTrp1rca4mdIXe"
                       + "wa2nAtH6xDHEYVe8sU1+xibZKAKrE0t3vaTrRk+2AFPXlvSUuFlWYUy5c+6Q1Roh7ZGr2m8zVNtd/NggquDdBgHK7b0TGBVd0puBLaBObcFWIcXCnM1gpjtycgCj2Q5uBvvuYWlshQ0+lAZ4J4Aa+6w3K4NDtTIVkTIFKYGzmofmjhn9jp8LCaS0DNHkvA9QZ3XipnUJfnxh4XKBRIRfrTkQozRMBKuNsAPRxi4RGUkzBiYD/fp+g0StWhAaCsOrG4kWnM3UcMmLBGy07GtiItyNE4ggq8wMhOqxAoqvVVgmZsqpAe7Qdi4WaDCDSQ0+lS8tU9jNC1fUBt6W2FYE4IiRtI637VVRDzeIxEmBF62j2AFnf720DZY9D9ueos32clVb81y6zUOQwHU0WXgG05Gl8YhpST15aISjNhfZynoIYeRW0shdxJEPJI98GwJh3XllQaisHyb7B8NwPQ1qGg5YPEyC8+15eL5UAMBsGHE0jbjDQWsMYchZrQPcZSadQMuS0u67rXbeTDOYeV8jGdVmmyHETP69haDxUpLFYbkhdNwSsi1j11gaIZk3gTZy14irOY5u9xht2tGsinJRfziaKcJhHH1EqxVFaley/jJZVLEx5t8v3CNgLCsas4CTubgjamsq0gw9YCGVIijEpQdxF5djHi6lbJY7qqY2dmMl912zSm1y099VCTFIyJ4cxYOV3xlpEvVDL1uHBbUjF5vQuCQoRpkidMc8jdfLhP8mQk9NRY6qwdKTU/0p8xE2dLXwOe1r5B2s2Rr4FHuKjJc6S475bE+r8ztnSXVfHSTbRq/gpNh+tafExq9gabHf3VrIhLAQm8kk2dOUAlmwVKVEh5YLx3dc6w1He2qqKqM7S93WME91s6AxJMuUpJUkYx+v4qwUoGlp7aAIdXsWC4WoL77droX8CFSUVX4Gauq6sARsLbp8DrUJgQe4GoQ0e6py/AGWrpw6SsBX+/rBQA9ZNxyBD5PYDUzrK/8s"
                       + "wfqT30DiPM5UbGrd0tT1QHf9+flSTh8lJFnL3WDAVBo4HdGpobNrAMiLLdVCa0edXBlfenWpfEffqgfhYipBNhfzWVHCl/xfdXdYWpQceseGokVv2ZFxWJNaLEnd+ha6tM6hBUh3ps5dYAeoc+nO1NnL7ABxNtmZNn+vHaDOZ3CmD19yB+qBM45mBEJnn/1mPMOV/JKicbqzIKLqoO5OPtsbqjv+OkrjNEUMb3Lp7qlzq8z2q8sCkL93zi/++LTRDAHWgtwL+qoL8SUlE+R1hZXbP+4WPLf109yu11IUrqnz20k+zWEdspbh0HxzAH59lZyDfP3NoYX0JjnXLPrBYSPfuS1xe3i1N9POgN2dYvSCteLAxgLTypJDCtdAq76CDZCrU7woHiopHjqBsbyMyaGx/OJiHhDuPfOGAiHRgS5zn5kjyXx3oMZeUebIsQkOcuvcXDnhqb1fdze7tCev/eYW+DjZZmJRlRxSuOPfgfLntvL5o+gEYzhrFLO72NHpqTRwsgj7zMiCcjqavFWd6ZVuyQ0RLwaVbseDHxFoeZ2nCVnqUBf085zeF2/viluKQjzSd0YP6zFksBs12SDjEHQUDXSE7Fxk2QEqk9Ktyq7kiBDZa2n70DX3peSlIWZp9VD9pf3femnUHhLml0wkl4kqCw3YW526E+S/5AVeliDZW/wez+OIaL4uw0eURPc4L27Sr5i+yrK//9b/OZQ2ikOeh/GY30SJqAiMISx6vj7CVbaBl0ySJ5QFjyiTQlwMEMIOpF1G7un7DslQXIsR/4aiK74iche59578gkhIfheDvSDiTU3xgog3PTjYvgD4/u+HmEfQK3+tYSP6SB2B3kcjmd9YGExnwE8oeINU9YQCRNAGq9zKwRLx39DzBRvBKvfswJLs2PvBHYgQ7zULK18WMM8K326s/Y0AwDiVH5r0yCsJgf/apfeaYsJvRNbqWO4+S0V1HPce1OQY7j2IQfHbe5DTxW4fUquOLG77"
                       + "RqD46jZX9ltND+JiMPTB1qNwrPMBwTq2OOcbASscn3ywTuLD0zZk/7JEz3/tGVLcf+PRhRQHm/lD74jhA26MRxJzeyPQg2JlD7dbBUJhD0acjXQNEv3JQ0/CgaxB8m88yEtxqv1WpABlOQz1UKSl0McDjqyRBHPezcbDOI87bdt8Tw30p1DwKYPyMM9mv3cLbPocRWs2M/WKWG0fuZQPZFFx4hdYlL9q7xG4sHc03F1EI31lkUedglPCURCYft16zMjdYMzyUoCnGWQzkBtVMMn+QcLLjDuL091TXW5A1SluKfocFzjhr5e6Vdc/vsDLfWMs7xIo1kpC7SGzOUAYXHO2rI1ox/R4zoMJV6fo4u08oDGQ0tyAotLdWvU+5fJGqJfa3P0jG+5hccVHK7cVmhYKmOQdgrdP+OWdhLMdYeBa/8jebZAytve2EHTbC8QDgUftJu9gvnECjwtcdx2d2+A7LIQgNL7asCHUtKYksf4N7hAVl/pt3QycEKNy9vawjW0ZNJBft+uy+xsCzTaX6U6g2fYSXXCpb7WdGDFQ7Dw+QHXVU52FlANR5Tp/PA3vUtLFlYUVjoQqkuX3axJ5PhmqRh9hVlddtUTXVlllMVVLc7lVzWxbtPUz+UxM6MJZi6xU04VUc/UZqgiMijtsFHio2oHixVuGiwclDAWfNzDArsGlitlEqEJNwFRV3Hld1HmoCnX4VKklzXpUbkaTArZBGc4WAqJCsXRJKkDCCmY7QcUhtSXfLJS1uzgdMyXBdypGGDhaN8qtoztvRRQ7icnPzipw6DqdKLxwNZ7A8sZe3EljR/IkhjTpa2LH2YrNGW1jjCguzGNccAqdIOSJVozV0r/xg8aUF+ZTLgCArqEOAur1pkIbwV33tILiYrNknhAuUGsbKK4D+GvXgzaxiSevbSJ8RbqXshuqiQ6B7OWr0GQXSdAWLSv/FrKNzaOHjgS9rJ/ggNs/tnnOk/u02csKHDVZJDfuAoVkc3mS"
                       + "kfUACgqSHBDElo/Nf0HxGlOf5jscnidX62K1LkiTyQo75oYC3Q7r6i+j9fM8H1X3AfIhmkDYjKgL61Xyfh3FYcv3GeA9pCBB99m10xjty4I6jz28tJQu08SSUC2+1jxwg5ermBDLr5IFesJq3swy5CV29CFCDxla5jWNrjz5S+AXLp9/+S9DtXcIvKQAAA=="; }
        }
    }
}
