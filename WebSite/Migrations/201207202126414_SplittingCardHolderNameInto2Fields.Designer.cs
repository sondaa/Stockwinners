// <auto-generated />
namespace WebSite.Migrations
{
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    
    public sealed partial class SplittingCardHolderNameInto2Fields : IMigrationMetadata
    {
        string IMigrationMetadata.Id
        {
            get { return "201207202126414_SplittingCardHolderNameInto2Fields"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return "H4sIAAAAAAAEAO1d23LcuBF9T1X+YWqekq1ajSRvvF6XtFvyWN5SxbqUR/ZWnlQQCUksc8hZkqOV8mt5yCflFwLwikvjSs4M5eRthgAajUZ3A2gAB//517+PfnlaxpNHnOVRmhxPD/b2pxOcBGkYJffH03Vx9/2b6S8///lPR6fh8mnypcl3SPORkkl+PH0oitXb2SwPHvAS5XvLKMjSPL0r9oJ0OUNhOjvc338zO9ifYUJiSmhNJkef1kkRLXH5h/ydp0mAV8UaxedpiOO8/k5SFiXVyQVa4nyFAnw8/Q3fLqIC771HBbpFOZ5OTuIIET4WOL5zZGr/J8rUtK2OVHhKGCuer59XuKz0ePo5xxmbg+T5O37mPpBPV1m6wlnx/AnfMeXOwulkxpediYXbokI5ysLx9CwpXh1OJxfrOEa3Mflwh2La5tXrt4sizfCvOMEZKnB4hYoCZ6RrzkJcNqEWxdvVaztp/DTbP6TSmKEkSQtUkH6WmBdYbaoinx+jkIjJwLQbtbM8X+OQF8eiyIhiTicfoiccfsTJffHQUj9HT82XvxHt/JxERI1JmSJbY2dmTpcoik/CMMN57lj5wX7v2j9EWV7Qn1tv90e0o4rP8ndE8XDb0+/SNMYocaaziO6TzyviHNom0N/X0dJPFh/T+ygZhNp1FqH49GkVZc+D0Fusb/Mgi1bUUDX+ouoMPamLtIjuoqC0+QUuCtLjudkFCTQv0GN0X5LQMDqdfMJxmSl/iFaVz96jJn7D5/qQpctPaVy7Qy7xZpGus4DKL1XluEbZPS78OMztWMy1POZmJnNnLikhmDuackNr4LjqvrZVNdwwSQ2jtlxAuqIWGZxbkByUCRYgmBOS49GsG8e1ozundh6jvGiFrqO9pRWPadRnWabsOnsKgd7JunhIs+if+OL0GhbH9gbek6CIHkspDOKj54jMa+PYiiDoqK1cFqUJmiDvNqUSnRlqM0qmqM89mDmWTPY0yUY/+5il"
                       + "nY6PwjSvsijolAwH0RLF08lVRn7V67s308kiQJRvd0NlhfIhw7+vyULxua/1n+Unj2SqTUtcp/UY5jgDtDKSlmGjpVQ6DxeFTUZdQms7mmKDGVHHe09L4jq8jzk5aM4obMpjNXToOA5Z9yzVIZ+OpOV8+q0p9/9uMrma2nP5TM5FFwHM272UBZx7eygPQMdHlxRkXoBqfcIBjh7xezJMPZ/EJMF9iIIpXpYO8SoKvg5FkQgr+Dokwd8w/jpwq6u+jE+I/Ndkerwk/ehI2n4kpOL4I0pIhfk5Xt76RXKrkj4635V8AWr+Pxh07BPl7V/7FcrzP9JsB8vcs/wjvkfBc20TmzG+eYbDqJgj0kAPo+tK+xgeX/oFGB9l9SGNQ5ztzgw7HnZmkBdrViEtqyU/ezsCGpcvO+o8TYoHRmUOXvcg9g+Msn605l++OErjB40sbDYDanc4XPz/XRTHhPHWzQJz5M5cb8Tc3ZRZmUmaQatz9ppQNzx5eDNGqK6uzLo/RuHHam4/Rgk+2EH4tqv9cNDabQxnXkrYqdIf+88h6GY5mUVfZouCCTBb1v+qf/1pXqB4TghsfvEtVv2QJthrwNDWbdXVZOVSZO6xT6WPrAmCzrHxXW2eziUKSZIjFNN7ub+mfp/JXCcv55mcrahH4f62MGlS9ddJnqdBVHKp2MRQBLX5Bp0m4cQvwl01Goqmk/au4yJaxVFAGD+efieJ0bnWdkcbrpUJ2fNVH0xFbb1M3uMYF3hC9/7oHskc5QEKZeMlcg/5L0TBcUZ1i3rAJC8yFCWFbA3EP0crFHs1UKDWJ0xPm9AyI6a8xyucUDvx6vyBuWyZETrAJO+jGWME9rYhb43aKKdmnxTWSV8rUG+xOpjcuPRe2SRXRYL3d701Xdmnw/G1Bd2Wz1Op1ExzuKpTrfIArIPuqs9jWRrG/t7egUTfS+eUrLj2Z08dU8p5GD52oVPiMljT/7mtVoEdryXsqFnfiVMp6/Yzu3gq"
                       + "9qAtvY6tcuvWwYqg03tONunauRbbhUJX6M/t9XQg2qN+DHH4hOHYBj9jk2wcgdVGrLtf0nWjJ1tAoGtLfkpcLKt0TLly7jSrDUHaa65qvc1QbVfxY1NRBe82GqBc3jspo6JLejOwBa1Tx69VmmIRzGZ0pttJc1BGcxTcrOy7V0tjK2z0Qxl+d1JQY5/1ZmVwVa1CRaRMQUrgrOahuTpHv+OnQlJSWoZ4cv5oUxd14oZ1Sf34wsKdCYkIP1tzIEZpmAhWC2EHok1cIjKSZgJMBvr1tQ2JWjUhNBSGZzcSLTibqeHS4Riw0fIRGhPhzk4ggqwzMxCqbQUUX+uwTMyUQwPcoe1YLNBgjEmtfKojwkxht8PFojfwjsS2IgAtRvI63rFXRT2cEYmDAi9aR7EDdxj00jZE9jxie4o228tVHc1z6TYPQQK37GThGUJHlsEjpiX14KERjjpcZCvrIYSRW0kjdxFHPpA88m0IhD2lLAtCFf0wxT8YhuthUNNwIOJhEpxvz8PjpUIBzIERx9CIuzpogyEMOat5gLvMpB1oWVLadbfVyptpBjPuaySjWmwzhJjBv7cQNGeUZHFYLggdl4Rsy9g5lkZI5kWgjdw14mq2o9s1Rpt2NKvAO+oPRzMFysfROVqtqKZ2Jesvk0UF+TH/fuEO7LGsaMwCTubiiqitqUgzdI+FVKpBIS5PZHZwI/NwKWWzXFE1tbELK7nvmllqk5v+rkqI2Cd7MjgJK78PpEn0eH3ZOiy4HbnYhMKtoBhlCkSSeRqvlwn/TVQ9NRUZLISlJ6f6U+aBQ3S18Dnta+TPjbM18Cn2FJlTvyw55rM9re70Lkuq++og2RaUg5Ni+9WeEgvLwdJiv7u1kEHmEJvJJNnTlPA5WKpSokPLhe07rvWGrT01VVXQnaVuG5invlnwGFJkSvJKUrCPd3FWDtA0tXZwhLo1i4VD1BffbtdC5whUlFXnDNTUdWgLbC26fA61CXgKXA1Cmj1V"
                       + "GVaBpSunjlLhq3X9YEoPRTccFR8msRs1rZEMWIL1Jz9D4k6cqdjUHktT1wNBGPDjpZw+SpVkI3eDKaYywOmonRo6u1YAebKlmmjtqJOr4EuvLpWhB6x6EC6mEmSDN8CKEsYueNHdYRlRcugdG4oWvWVHxmFOajEldetb6C4+py1AujN17l4+QJ1Ld6bO3tEHiLPJzrT56/oAdT6DM3347j5QD5xxNBYI7X32G/EMSAMlReNwZ0FE1UEd1ADbGyroAh2lcYYihg+5dNfvuVlm+9VlAshfp+cnf3zaaEyAjSD3Un3VPf+SkknldYWVyz/ucj+39NOABmgpQrfv+TUlkMGHPmwQULrDfGctq13zzcHAxCvwnI2JiT50q9vwMNkqzUGi9F48J0L6wSEw0R3D4mIS6tNZOzPUbleml5kqNqAsbFRZckjhGmjVF8oBcnWKF8VDJcVDJ2UsL5dy2lh+cQl3CNe4+cCHkOhAl7mezZFkvjtQY29cc+TYBAe5dcd2OeGpT/PubrRsd5L7jZXw9rjNQKkqOaRwx7+i5veh5f1U8VCPYe9UzO6yL0B32YGdUvgMkCwop63WG9UeZXnMuiHixaDyGPXgWx5aXudpQqZu9Ej9WU7vv7d33y1FIR5RcNYe9gSUIQ7WZIOCXdDWOtAR8mEpyw5QhchuVHEyRw2RT2FtX3XNfSmdOhGztH6o/tL+b0+d1Cc+zA/OSEdAqiwUV7k6RUA0/zkv8LJUkr3F7/E8jojn6zKcoyS6w3lxnX7F9PGc/f03/q/WtKgUeR7GY366JqIiMEJy9HwkhqtsAw/OJI8oCx7o2kSA7BgAaRCkXeIQ9X0uZiiuRRy4oeiKj73cRu69Jz/0EpLfxWAPvXhTUzz04k0PfhNBUPj+z7yYLeiFP6qxEX+kfijAxyOZn8IYzGfAL114K6nqpQuIoI2ucjMHS43/hl6Z2Iiucq9DLMmKvZ+6A0D+XqOw8gEI86jw7T6JsBEFMA7l"
                       + "hyY/8kJeKnjp0ntJ0P0bkbUact9nqqiG2+9BTYba70EMgtnvQU4HsT+kVx0ZvP5GVPHFLa7sl5oexEXM+sHmozAk/YDKOjY4+o0oqwZGfij1UqPED1UDj+nbUP3LEj39dShg93yJ6GU4dwcLY7t7k2Pg3UHp/dAbvX3AZf1I8M83YjgQbvlwa20AltyGuCvqOEjzRw8nD4OKg+RfeZCXMMP9ptMAZRkS3IK0F+L3gHY1EmDt3SyajOOD05LTd8dDv4MG75AoNyJt1qo3wILVUbTmEFkv9HB7FFkeVKTixA/klYc98ACR7I1MvAtk2BeGAusEFAojUjD9unX8zt3omOUFDc8QzmZUblTAnv0B28uMO8NM7+kuN+DqFDdGfbY6nPSvl7tV1z8+EOy+eNe7VBRrJ6E+3bM5hTAcK9qyN6Id0+NpFQY6UNHF23nMZCCnuQFHpbtB7L1D562hXm5z9w+euEMUi++ibgsmGAKv8oZD7gOFvRNo4RGCCPujrLeAcWzvbQEA3UuJB1Ie9RF/h/CNk/K4qOuukdIN554FOEjjCxob0po2lCTWv8EVogJgwfaIhJPGqA6qe8TGtqw00Jl012n3N6Q025ymOynNtqfownWA1tuJ6I1i5/Fg4VVPdRFSTomqY//H0/A2JV1cRVhhVFqRLL9ek8jzyVA1erRfXXXVFF1bZZXFVC3N5VY1s2zR1s/kMzGhgxYXWamGC6nm6jNUEYhQPCwiP1TtQNj9ltD9oIShhwAMDLBzcKliNhGqUANeq3oDQPcCAFSFGspWakkzH5Wb0aSAbVBCC0OKqHAsXZJKIWEHsx2Ad8htybciZe8uDsdMSfDNkBGCeOus3Bppeyui2Mn7COyoAsMI6kThpVfjAfk39uJOGjuS50mkQV+D42crNmdtGyO6uzCOccAaOkHIA62Im9O/8YPi+wvjKQdeoGuog4B6vW/RounrnrlQXMqWwhPC5W9tA8V5AH9lfNAmNtj+2ibC"
                       + "17t7ObuhmujwqIB8jZusIom2RcvqfAtZxubRfUeCAg0kOODWj22es+QubdayAkdNFukIeoFCsrg8ych8AAUFSQ6IxhKXMJ18QfEa0/PYtzg8Sy7XxWpdkCaTGXbMmQJdDuvqL19O4Hk+qu4y5EM0gbAZ0Wt6l8m7dRSHLd8fgNNDChJ0nV0fGqN9WdDDY/fPLaWLNLEkVIuvDQ9c4+UqJsTyy2SBHrGaN7MMeYkdvY/QfYaWeU2jK0/+EvULl08//xdr5fXfH6cAAA=="; }
        }
    }
}
