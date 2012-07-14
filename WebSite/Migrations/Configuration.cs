namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using WebSite.Database;
    using WebSite.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<DatabaseContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DatabaseContext context)
        {
            // This method will be called after migrating to the latest version.

            // Ensure default subscriptions are available
            Configuration.SeedSubscriptions(context);

            // Ensure default roles are available
            Configuration.SeedRoles(context);

            // Ensure countries are available
            Configuration.SeedCountries(context);
        }

        private static void SeedRoles(DatabaseContext context)
        {
            context.Roles.AddOrUpdate(new Role() { Name = PredefinedRoles.Member });
            context.Roles.AddOrUpdate(new Role() { Name = PredefinedRoles.Administrator });
        }

        private static void SeedCountries(DatabaseContext context)
        {
            string countryList = @"Afghanistan
�land Islands
Albania
Algeria
American Samoa
Andorra
Angola
Anguilla
Antarctica
Antigua And Barbuda
Argentina
Armenia
Aruba
Australia
Austria
Azerbaijan
Bahamas
Bahrain
Bangladesh
Barbados
Belarus
Belgium
Belize
Benin
Bermuda
Bhutan
Bolivia, Plurinational State Of
Bonaire, Sint Eustatius And Saba
Bosnia And Herzegovina
Botswana
Bouvet Island
Brazil
British Indian Ocean Territory
Brunei Darussalam
Bulgaria
Burkina Faso
Burundi
Cambodia
Cameroon
Canada
Cape Verde
Cayman Islands
Central African Republic
Chad
Chile
China
Christmas Island
Cocos (Keeling) Islands
Colombia
Comoros
Congo
Congo, The Democratic Republic Of The
Cook Islands
Costa Rica
C�te D'ivoire
Croatia
Cuba
Cura�ao
Cyprus
Czech Republic
Denmark
Djibouti
Dominica
Dominican Republic
Ecuador
Egypt
El Salvador
Equatorial Guinea
Eritrea
Estonia
Ethiopia
Falkland Islands (Malvinas)
Faroe Islands
Fiji
Finland
France
French Guiana
French Polynesia
French Southern Territories
Gabon
Gambia
Georgia
Germany
Ghana
Gibraltar
Greece
Greenland
Grenada
Guadeloupe
Guam
Guatemala
Guernsey
Guinea
Guinea-Bissau
Guyana
Haiti
Heard Island And Mcdonald Islands
Holy See (Vatican City State)
Honduras
Hong Kong
Hungary
Iceland
India
Indonesia
Iran, Islamic Republic Of
Iraq
Ireland
Isle Of Man
Israel
Italy
Jamaica
Japan
Jersey
Jordan
Kazakhstan
Kenya
Kiribati
Korea, Democratic People's Republic Of
Korea, Republic Of
Kuwait
Kyrgyzstan
Lao People's Democratic Republic
Latvia
Lebanon
Lesotho
Liberia
Libya
Liechtenstein
Lithuania
Luxembourg
Macao
Macedonia, The Former Yugoslav Republic Of
Madagascar
Malawi
Malaysia
Maldives
Mali
Malta
Marshall Islands
Martinique
Mauritania
Mauritius
Mayotte
Mexico
Micronesia, Federated States Of
Moldova, Republic Of
Monaco
Mongolia
Montenegro
Montserrat
Morocco
Mozambique
Myanmar
Namibia
Nauru
Nepal
Netherlands
New Caledonia
New Zealand
Nicaragua
Niger
Nigeria
Niue
Norfolk Island
Northern Mariana Islands
Norway
Oman
Pakistan
Palau
Palestinian Territory, Occupied
Panama
Papua New Guinea
Paraguay
Peru
Philippines
Pitcairn
Poland
Portugal
Puerto Rico
Qatar
R�union
Romania
Russian Federation
Rwanda
Saint Barth�lemy
Saint Helena, Ascension And Tristan Da Cunha
Saint Kitts And Nevis
Saint Lucia
Saint Martin (French Part)
Saint Pierre And Miquelon
Saint Vincent And The Grenadines
Samoa
San Marino
Sao Tome And Principe
Saudi Arabia
Senegal
Serbia
Seychelles
Sierra Leone
Singapore
Sint Maarten (Dutch Part)
Slovakia
Slovenia
Solomon Islands
Somalia
South Africa
South Georgia And The South Sandwich Islands
South Sudan
Spain
Sri Lanka
Sudan
Suriname
Svalbard And Jan Mayen
Swaziland
Sweden
Switzerland
Syrian Arab Republic
Taiwan, Province Of China
Tajikistan
Tanzania, United Republic Of
Thailand
Timor-Leste
Togo
Tokelau
Tonga
Trinidad And Tobago
Tunisia
Turkey
Turkmenistan
Turks And Caicos Islands
Tuvalu
Uganda
Ukraine
United Arab Emirates
United Kingdom
United States
United States Minor Outlying Islands
Uruguay
Uzbekistan
Vanuatu
Venezuela, Bolivarian Republic Of
Viet Nam
Virgin Islands, British
Virgin Islands, U.S.
Wallis And Futuna
Western Sahara
Yemen
Zambia
Zimbabwe";

            foreach (string country in countryList.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                context.Countries.AddOrUpdate(new Country() { Name = country });
            }
        }

        private static void SeedSubscriptions(DatabaseContext context)
        {
            context.Subscriptions.AddOrUpdate(
                new Subscription() { Frequency = SubscriptionFrequency.Monthly, Price = 39 },
                new Subscription() { Frequency = SubscriptionFrequency.Quarterly, Price = 110 },
                new Subscription() { Frequency = SubscriptionFrequency.Yearly, Price = 350 }
                );
        }
    }
}
