// <auto-generated />
namespace WebSite.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    
    public sealed partial class AddingPickTypeToStockPicks : IMigrationMetadata
    {
        string IMigrationMetadata.Id
        {
            get { return "201207242137053_AddingPickTypeToStockPicks"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return "H4sIAAAAAAAEAO09227kupHvC+w/NPopOUDssU8yOTmwE3g8nsDYue145gT7ZMjdtC0ctdSR1JPx/loe8kn7CyvqyksVWUWpL57dt26RLLIuLJLFKtb//PNfZ3/5tkpmX0VexFl6Pj85ejGfiXSRLeP04Xy+Ke9/99P8L3/+9387u1quvs1+6eqdynpVy7Q4nz+W5frn4+Ni8ShWUXG0ihd5VmT35dEiWx1Hy+z49MWLn45PXhyLCsS8gjWbnX3apGW8EvWf6u9lli7EutxEybtsKZKi/V6V3NRQZ++jlSjW0UKcz/8m7m7iUhy9jsroLirEfHaRxFE1jhuR3DMH9eJPclDzvruqw6tqYOXT56e1qDs9n38pRK7WqOr8h3jSPlSfPubZWuTl0ydxr7S7Xs5nx3rbY7Nx39RoJ4dwPr9Oyx9P57P3mySJ7pLqw32USJzXL3++KbNc/FWkIo9KsfwYlaXIK9ZcL0WNQkuKn9cvadT40/GLU0mN4yhNszIqKz5bgzeG2nVVff4aLysyeQbNg3ZdFBux1MlxU+aVYM5nb+JvYvlWpA/lYw/9XfSt+/KHSjq/pHElxlWbMt8I9mCuVlGcXCyXuSgKZucnL0b3/ibOi1L+3Dneb6M9dXxdvKoET/ScfpVliYhSNpyb+CH9sq6UQ4+C/P05XoXR4m32EKeTQPucx1Fy9W0d50+TwLvZ3BWLPF7LierQFw0z6KBoI6RAfZ+V8X28qDXJjSjLSo4Kv2IzYL6PvsYPNQjHmOezTyKpKxWP8bpZCY6k4rjVa73Js9WnLGmVrFZ4e5Nt8oXEOcNqfI7yB1GGjbCgDbFwjrHwD7Jgj1ICgkcnS25lD9qohq99V91olKJuoNRRQLKCkwyubVAOqgQTEKwJ0fHseNgdOPcMmtgF7B3Muc3dQxB1wyHtJdQhy+GyNYUB72JTPmZ5/N/i/dVnmBy7W84vFmX8tabCJJr/Mqp2y0lCAkhR1Je5WMblZZQv"
                       + "t6OeJURwMusK2GoxTGhnRWtSu2tzFeRAHT8Oal1k9EMV97iVepOpopqsI9VRNzfHqCTa/D4ItfQxjxfDBBOLeBUl89nHvPrVnph/ms9uFpEcN19JqUR5k4u/b6qj99NYzXddXHytDi+yxeesXb+Ze2rStO4H7J0XzSyFm8LTBG/hnDWOZpNNomHsI2eSxvAx04khOQcxpwLOl6fMNZjMWUVjB7BTXzm5POSsuwfBODnUxyxZinx/BophDHszVbzfrO4GYxex2+rnaJuUPJrXjHqXpeWjIjInL0cA+y8R5eNgXf7yC5Mav3fQgrJlbS1z0+1XX8VJUg28t/gBK5qyIzNrD4sYWslat/Cao5aqbkwB2kwhKleVkflxEHqsHe3bOBUnezgNDr2fTto76axXU5jV6R9HY1xb9Ksz64f8plTOq8T+fxzff1aUUXJZAdjytgPo+jFLRdCC4eybxOpsk5Y5/ziBn4QbgKBy7HRXX2dQiUaRpQjN8lHqr+s/ZDM30Iu9k6OS+iDU3w42TWR+SREIYZZsF8Kprt33yaYAfYXO9tZyEHIxYM5w4M4gSFhAu3+A8ABwQmQJAfMMROuTWIj4q3gdxcnTRVIV8E1EMMQPtUHiY7z4dSqIFbEWv04J8G9C/Dox1g0vk4uK/ptqm7Oq+MgETbdESXL8I06rDot3ot5WBEyApmWIzA8tn4GY/x90oxjjtzK+949RUfwjy/dwxXZdvBUP0eKpnRPbmXyDvgyZdEPrkImnt34Gk+8yS8uqs92byS5zMd1d68fNXRIXj1Ncs14XLbAAJyd0l/YmftjkiAPHIDK3fbVhv2aXWvs2oMqo/VsDJGTuNC1D5s3Q8hnMmVdJdvfXTbz8ksdcI8nLLR3Q5NZrPru4K8o8WpQt7AAOSjgh/OvaPQPu3Tyt7rKEyTgm34CFQSgebrvWta1Gq3rbt4bc5iJwmWRFKIpBmlsKPaSz1e+WttYKuV4uX9bLCiPHaPoKxmja"
                       + "7/BousJRq4aE1AAK1TtN61DtM7R+Bjpor9qg9tgPmHFkURjMC/PZq6gQLYKdkM6Jwl4dFWBJH+DfNnUGYTeKLHk3y0eJ/ACsghUi9RqAEMG3ADwD2Z/6wE1Zsq6k3X+7TmJX3+IyuAcKDv+5iVoW0e+JeAYr6oz9Donb4x/k1IyqMNyntuvv1nKi1UpsPzq9eJy/nIp0kJ+cSTW2fxyP7M9Vf7EtZwFLUCgPdQjjFqH/5yKJixdFkS3iepiI2zXihqtjdJUuZ2E+uYN2t935322SMl4n8aIa+Pn8B4uO7F77+CO4V8XJWO/6ZG4K74f0tUhEKWYyUkNq/MuoWERLWx9XdF/qXyp5F7kULulgkkorSZyW9uSI00W8jpIgBA1osMKkORZLFPrBmCWvxVqkcqIEMX/iUfaDMRjgo/fZsTIJ6HPDDj+hCKcjFgWWydBZgIexMKbcYck9ihJXkODVJVjSUZ5ON64dyLbpj4UJGeqcNYhVf5tIF13MpUuB2juKHZqMImOncB/1IGMJI8KS0QPYgdThLtKYpBD8pRWZUcLr6MLod7T2C/v+xdKLBUU+UA9vloB6eTZ6KLte/NEoIGQ1hoI4J17wgfhP0kzYv6R60CDpMUdcVfjCbnNtirHsQFbtpywwEXK8azEIT/2iEUMu8acwiEL/4ujoxIIfJFvoULibs5FyhdJ5mnHsQ6bwNdr1EIlbqkDGOwEzJesH0y5Cxl9xYsaGB3k0D8OqPdcZswh6OIU1J7nMJXhLG6xwP5kyUoE4X1lRgMOPuxzaIudFiaIISH7ofL3kYmPgsPa3TwOc2DAxc3m0DfKlOHHytJXDG04B3/q4TTaVNVcQbGiwX4hx1cbDFvQm2TaenZOJc1CWx8l4PE0/FQNk630yGa6mmwE2NtTnYBie6hHBwhvzVwBhSy+EybA3bijR0xByXalsDIarZcYhC77mhMAeqA0VRIC073RfqvIOViB3JhrF5MtKc1smPdKrFiLv"
                       + "l5bmBdnaU/2b6dPftKnWPz0ebrh40zZDlrzpjY1H/iwg+h6XAUzC8AFs5JgBtLuaib2glTs2D/zhGA3BVO0ZHkCt4QgcWm+98w2mtpPC6PWGaQ+M9uFEq31zLvA0hje5Fiy4mo+TVogYyEU7kMwHWAsXtACq+yoPoN6z1QLS7So8ANp4QKt5sxgQGvferCCIbr33ANLWRwiWsYBS+NYpRpRlw8JEHh0Gz3DxMAEquhTXPdizXkpjRCNRPBFQ8yjBF6GnB6gwrUUn2PsA6UfToeZOQCctk+zAS4luanvutnEbNHq7jeBMpyt+n81hWwAhrXcqbNI5r041jLDLUwUHZbVykAa7L1UAKUvWaCI4XjKyyUG802Pe6qmYqTsDB5H893gUuo+dfOpOxTPtsFsl1r3S+KkG3CTRqB9AKuDRbZtInusM4oWGgkO7NXcQBL/CoNJ3CmKA88tniKea4sfRA5xEUxNEfTjEJgRmkffZ5JUBt3tyB+KAFd5HuFDOw5t3RAD8xnqmuZ4vDk4DvQKOdCjh0wyKVraJ5TMHUw3CCj7ascZBJIf9V4HWj300QfQgQJsUuGXYbxtWBtyepRyIg6bg7aHcn80QlEEjsd9MzEfZtAobEPpxjkbbCjuzMXeajElGYxb+mHVYAWKefsdvs4zIFWB35bAcU2zHLBIghmJ1eTQO7GQKdO77vUGyLzs7bhJetR/OjpHMWGfvovVaKt6hZftldtOkybr83Q0/GdaqgXG80ATMNJ/2PZVZHj0Io1QeVZaifnpnSNF1uVxZ1Yjm16431Qprc60zbXS15e+mhZkv7MhO6KXS702FknzAqcZOGKuo3WwmU5RFSZQDr6dJ4/ZllmxWqf7NFDocip1gS4Vnl4ZD1pNtuXrRa9J71F8mUnvQS+gQleedVHDKZzqs4cEmFdTwlUHZPpGVRsX+Kx2SmspKhaV+52GoZLMy0VSK6DCtnFYqVKuQgbnhIaVh7/GeokHFBo3VofeC"
                       + "eU+ovVA9LOQKYOgl67LM0n3WraOuSElq1nceZahb10GfoHbdzfcnQN3tJQYZu93EobsyFqm9uOoxejNyEmk9GGV0qHZqIhWuXcqArLnhalCdDroHMYWaDeJk0wja/TKnEgxiP4Lfxt+rANtPYVNTiyzEhukMP8T7gZLr6Ou8XX6QIqneT00mmOg1HlM6HXD2LQD2JhHbIO6JyapBfxRj8esLAjddjadX8fhyBCSs0VckoEIIfPj8AJUzRK1NG6AJ2wZy1XBBsbLGaEcvszAEbpNABgbblDEoKlPJaCSUHxjbmiGsTNvR4NFme5uowxXlqGmK3MYS5ijackriemC1OVgAcG1JEMRTFOIpSxhj+WKIJo31F87Wxsh8om9yjEIGXCWjiQZS+c6ApiYp0cCpBQy6DWHIGvHw6OT9rZa9W8W4tRL2FaEslFjLKYn7bDczzYXuKNbYGUZIfIGbYYTs0oqopIRTlDxrdhBvqRncoUAkcIsGhmGyI1jseLyFUm5o0gKUs6Fr6TcA6Fo5G7qaigMArhazYetZOQDoegU2fDhFB9APXPFgZiDk3D3u+O5JKFJD9J7dCUAwBg0ZRVRuYBlKXJAO8z5o+nuvIcuGtlfrv3KsWXrWDN2SpZcdzBRQVeQo0cfSedSQfCLvaoyRW8/hoRLblRvEBbFPsqFvBduPLOs6ZrPXShhCqibO0ORULeCIqvI2vC6nSsHBCCnul8UQUChfRg3FJ5xYQ1xzdkkydMUJJ91wQdIyWKjAtIKDYZPqxzWKVdgj9SR2uRqj80t7mV6bYI4X710QtafjNeWkFnBmbP8avD5f+88HJQYjBUAJxOYLgLovZ0kAxPvt2FeC9pWB5FBOEluiBrpZU14c17ZqyneOibx/XFy3jvefGRecZti0dq/li6neOv+vC/n7w/1vQM3420CluGv2dxl8NNq23/anSM0kO8Buqi/b/oZqd1tILe2OBlAtOJhFxHCInmg1sRKA1MDoCwrY"
                       + "HqO4lfVDpbonp4gLLt2w+fwU85A9QwU1fD0Y+TTc1adZ3UNdkdztt7UIPlvDuxnMP5FyCeSeD4BfvUD88+WLeB4M1IM+7OAFMyDUE6hgVue4x8pgFiAsAY4ftQnFimu4xQIC6mePOiBBA0RfOJrc89c51sssXcay5ey6kMk5+sQcRFKY8UBs6VGjZz33nV016FITimMBGGEH2hIZgF2F3mL3oUwJsSN4dy+6o3kJBbuSLNVDdZrNDyAgHhVLJCTFjH3rtmgzeY6H3hKHzDVwjtcERFKMliQ9SthjoZxAeqBwYiITsDP6LXZQZ0oJFKd8sPLhQHwamejDk30y0Vekm6Ux4hsx14ckF0YwN2NoIVb2w5YPK96cdFho67JMDQA3kPj0EbKiQJxIYpDwd+IYx1lRxouOlx5+AbIi1M0q/TGq/dL/7yPU2+hwLWy9ppgMQq8pVbSR6ma4eFNFZuBsIo6lDbYoxaoWyqObvyeXSVzfb3cV3kVpfC+K8nP2q0jP5zKafT67SOKoaB4OYAXC9yn/imKZAGHwcq4oO0ojbJ2UGRE7IPjyIXbtmjf8YkkCb75DbgpnK6Zc6WwkLD1yvIGbfo3yxaOMETCy8nLzuGq+NA7Yda5FJnDFg2jaUQ/+RNPCHaLNG7h3MZ97apR5A0UudqU/mzeMphJfPhKaFVc+Ep5pFQAFnpTqF4kdx8dHgYrFilPnJT1pL55ghpaz12ldIaTsxfkwtZaD4lXD9ZwrIHtiTWRGZo8UfTsge5ys6nF64RTVzDLE+Rgk57b9mi/rsLWaJ++AEE4t81qW81WWinHTRgufHbFHAOKlqWtWEMexnMdstjvih3m8xyg5tQB4NxqnPn1EJjiaT49EZVe0r5+0qA6amp5g7PC0+zgofnjaHrrgPh3qb1bRt9/y9+BmMHEDtFhF8rVevm4wo4hHgqsjiR3U+z13tVPiXiffjMF5U0nTBw3H9c8dGKOpJ44e4Dv1DkmL9aUDJ21v"
                       + "amwdMP/IH+9HMwjYAf7HAPBKQPAY1Q9AVmODyaBJZB5CWSefV3A+bNqyhEXYEtYkEKOdL/De9YFMRjs6lkZD+A7YT8Cu3fexPSJkZiRRkxSU6icuAmaLtIaCXcMtVVBw62hoajTraGB68OpocHCM6vTHJk9UJ01IsUhOv1wOLbcois/Otku3dAcAH4JIJ96WmdGkkwsrFo9JE1K3i4ZPUPXW2zxydpGdUxzXdLf8kZZELapznBFRiz+YXE4gRxeajGDeIH75GFpuUTa0AEvXIeIlGzTqyxR8VaJdTk9t4MX8Vmhsdjl2+Fmtt94iu7VYoinUgRKXOVYXbJu5oWwNZei2b6PaCDPHpA2Ys1PLhxlptgclv+U1TAsr46BHFl6HrxRNij3eQ35xtgAcjj2CIjZquFfLIbGIV1EinYGqX0WdLOTkpwqZRSSBngZYnfsgMH4PFByG0LDwazPAxWvya1I8RIt4WeaOxyJckpkA9imr/kNPgBYIJa0vVIqjB74z4sIhLnTrJB6mAvvxodE+FJPmLWDXnNxVYhpRDTJQTrjpmtCkdfDoUNa9MA8Oa3GzwJAc3RyKOYhPijuyKtYOjzQ412k3EisXa6VtNkkZr5N4UXV/Pn9xdGS9LWzkQ9RgNR90GD9YACpJEbnUkfLGLS3KPIrt+NKK/OkiXkcJNH6jMryueqMY+y7MktdiLVKpxVU8p+mzB23MDR9NNF90gkwQ7k4GPsKpKhW+whV0Pp9YQQ0f0tciEaWYSXc8uR28jIpFZL+5XLvyH4KMEV9ZDbxh2o7IUXMk7UT0GOnUHZnkUfXUFO5E7ILU5RZUnU04RA4ouWUY8jdK3eL971oGUQ87KF+6xmf183cvcc4cJjzPw+1Kma/nXS2vjsRv3LXLs8fap1iQFyH8aYDtiYLnTYIdaxrJmFuKAzXM2aE2xmKlxs7VEXtR3sJC6MqZFeyBHiyhQcuyZyA7kNhBfd6+iqV78APsQDuIgeFBUXO/"
                       + "/7YTQWSvzxMJH54wiOFLzBIw5irs7HQHwtT2fwv7iioMbMs17nXfdiJCDCGeakOFJrphOMyyhIcjrs4edyA5wDs9GOsUfyWVe+pn1naq9WtRYXWftiIIzsfzeQ5WLHFA30R39el9jWgHsqG9v4NxsfZuUHnYfDhoSUAeeaaaz7fFfc97ILviOPi6/MQcVxyeTEjd52fPeecD+4fIffPhHIx5gNt6zT3tO0scdC8TGGZdshWhcL5lvx3Z8L2W7LpA36+QeF5ZVJhaR9yovGw+7GSb2d+pm/1v8coCSdtHDSliyQ/2LCban+Mty90KDfQwFddO9x0JzS7teiyh2b9NT9F4kh8DuNs3eba6bQqyW9zhg7lloS1Q45a8g9+0TLcW7sLq23l1OAUE9aCZUD7s2LvGOKt8/h6kw5Xm5rCF4wTlve41CnJvl5fdXEGayr7vSY/AdpbdplwRup5cwK701yV7853xEqQlZu2ro8qqOp8NLmXaJqd5RfJ8vrzLKuY3LmntA0OW4tHB6heQFni9GOrGeLOf0V0zN5xdNlV83cpavK6Vezhn/0o93yC6qjFhKOq1h9W/Wgh1OpT7O+rt41YvfQnURVtIwaS7ArDR6EpAHPpM9L4OmpOfBb35DIFusqj7wMK+cFY3cDWoW7imTyTtoHNAHoFKoDAC2ZJ9A1At8FbHaiHUoSN9t9VRZ5S1OukKoA76B9w9wFXbn9WBWgh1MpTTOkK6wIH7wRqWKgu+UQ51ZOTjIgnesD+BZW4oR8Wtq8LBEenSrODGktbpcAIHFQi+dnalHLwg/aGVujFiMA3qSS10ssvuR38fG/VJnyn1lM2Hw3Fd354CGw07q4+9HzMNPErL5oO5RdbRoaIK628EZYqLsdczW0GEsnjshBRu11+AHAxfYVQaVIVgSURT6CBFkFyNJY26R/MQBfcHQdxjFBTUzwdAAihHF0EpuKYGSZD3zm+HX6CH/VSPQhRF66Ri4arUIJKNPeEC"
                       + "yIc7qQEUI3q0aQgZR5saj/6bgxDsqRaAvOlUBaDs9LvSB6wfsZrRdt8ciDIIFIAilKXLxtLnI6QN2D6M1GNWPzvQ1U8Yap6n8chq7iwAmri7C2ArVgbYfNgnUv3JB0EK9OgYiZR9XDMzMI1HzsrkY+Pn9FnAr3CUETs270h75XwH5BMaj7Z9xQ4g7s12aN3EKyNuPjiQNc9cei7GSVFsL4TdKMLpjEZtQbaNYsD1pVu+gy5BR87ygEkTskvj3uNB+7VRd4EjyWQf6vWc0dMT6cRPA/uGCr30gsbsP7NNiLSVvasvOztuDCHth+qvlaXr7PhTtZGKV02A/tlrUcQPAwiZgSwVC+0+p69znd5n3f2SMaKuivXEZxlVy1t0kVen/WhRVsWLajNW7Xbns1+iZCPke5d3YnmdftiU601ZoSxWd4m2y5PXU67+z46tMZ81M66YAoVqmLFcoT+krzZxsuzH/QZ4/gABIe+92udWJC9L+ezKw1MP6X2WEgG15Ouv6z6L1TqRm5UP6U30VeBj89NQp9jZ6zh6yKNV0cIY2ld/K/Fbrr79+X8BGUde9F4YAQA="; }
        }
    }
}