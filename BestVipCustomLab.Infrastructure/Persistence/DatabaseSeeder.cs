using BestVipCustomLab.Domain;
using Microsoft.EntityFrameworkCore;

namespace BestVipCustomLab.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (await dbContext.Campaigns.AnyAsync(cancellationToken))
        {
            return;
        }

        var trafficSources = new[]
        {
            new TrafficSource { Name = "Instagram", Slug = "instagram" },
            new TrafficSource { Name = "WhatsApp", Slug = "whatsapp" },
            new TrafficSource { Name = "Evento Geek", Slug = "evento-geek" },
            new TrafficSource { Name = "Indicacao", Slug = "indicacao" }
        };

        var categories = new[]
        {
            new Category { Name = "Canecas", Slug = "canecas" },
            new Category { Name = "Quadros", Slug = "quadros" },
            new Category { Name = "Kits", Slug = "kits" }
        };

        dbContext.TrafficSources.AddRange(trafficSources);
        dbContext.Categories.AddRange(categories);

        var geekCampaign = new Campaign
        {
            Name = "Geek & Anime",
            Slug = "geek-anime",
            Headline = "A proxima colecao premium geek da BestVip comeca com a sua voz",
            Subheadline = "Explore produtos, responda a pesquisa e ajude a decidir quais itens entram em producao primeiro.",
            Description = "A campanha Geek & Anime valida demanda real por itens premium antes da fabricacao. Tudo no site gira em torno desta campanha ativa.",
            HeroBannerLabel = "Campanha vigente",
            Status = CampaignStatus.Active,
            DisplayOrder = 1,
            IsActive = true,
            ShowPartialResults = true,
            Theme = new CampaignTheme
            {
                AccentColor = "#d9a520",
                AccentSecondaryColor = "#1be2ff",
                SurfaceColor = "#101322",
                GradientStart = "#1a1032",
                GradientEnd = "#090b12",
                TextColor = "#f4f0e5",
                MutedTextColor = "#98a3c7"
            },
            Assets =
            [
                new CampaignAsset { AssetType = "hero", Label = "Banner principal", Url = "https://images.unsplash.com/photo-1578632292335-df3abbb0d586?auto=format&fit=crop&w=1200&q=80" },
                new CampaignAsset { AssetType = "gallery", Label = "Colecao premium", Url = "https://images.unsplash.com/photo-1511512578047-dfb367046420?auto=format&fit=crop&w=1200&q=80" },
                new CampaignAsset { AssetType = "mockup", Label = "Produto destaque", Url = "https://images.unsplash.com/photo-1542751371-adc38448a05e?auto=format&fit=crop&w=1200&q=80" }
            ],
            Contents =
            [
                new CampaignContent { Key = "primary-cta", Value = "Quero validar a colecao" },
                new CampaignContent { Key = "secondary-cta", Value = "Entrar na Lista VIP" },
                new CampaignContent { Key = "story-title", Value = "Pesquisa premium com tema geek, sem perder a assinatura BestVip" },
                new CampaignContent { Key = "story-body", Value = "A atmosfera da campanha traz anime, colecionismo e energia neon, enquanto a base visual continua sofisticada e premium." }
            ]
        };

        geekCampaign.Products.AddRange(
        [
            new Product
            {
                CategoryId = categories[0].Id,
                Name = "Caneca Black Gold - Solo Leveling",
                Description = "Caneca premium com acabamento fosco, interior dourado e arte autoral inspirada em Solo Leveling.",
                PriceLabel = "Aceitacao media: R$ 79,90",
                MockupUrl = "https://images.unsplash.com/photo-1514228742587-6b1558fcca3d?auto=format&fit=crop&w=900&q=80",
                CtaLabel = "Tenho interesse",
                InterestScore = 92,
                DisplayOrder = 1
            },
            new Product
            {
                CategoryId = categories[1].Id,
                Name = "Quadro Metalico - Jujutsu Kaisen",
                Description = "Quadro premium com brilho controlado e composicao mais adulta para colecionadores.",
                PriceLabel = "Aceitacao media: R$ 149,90",
                MockupUrl = "https://images.unsplash.com/photo-1515879218367-8466d910aaa4?auto=format&fit=crop&w=900&q=80",
                CtaLabel = "Quero ver este lancar",
                InterestScore = 88,
                DisplayOrder = 2
            },
            new Product
            {
                CategoryId = categories[2].Id,
                Name = "Kit Premium - Naruto Legacy",
                Description = "Kit com caneca, quadro e embalagem especial para presente geek de alto valor percebido.",
                PriceLabel = "Aceitacao media: R$ 229,90",
                MockupUrl = "https://images.unsplash.com/photo-1526374965328-7f61d4dc18c5?auto=format&fit=crop&w=900&q=80",
                CtaLabel = "Entrar na lista deste kit",
                InterestScore = 84,
                DisplayOrder = 3
            }
        ]);

        geekCampaign.SurveyQuestions.AddRange(
        [
            new SurveyQuestion
            {
                Prompt = "Qual universo voce quer ver primeiro na colecao?",
                HelpText = "Escolha o tema que mais tem potencial de compra para voce.",
                Type = SurveyQuestionType.SingleChoice,
                DisplayOrder = 1,
                Options =
                [
                    new SurveyOption { Label = "Naruto", Value = "naruto", DisplayOrder = 1 },
                    new SurveyOption { Label = "One Piece", Value = "one-piece", DisplayOrder = 2 },
                    new SurveyOption { Label = "Jujutsu Kaisen", Value = "jujutsu-kaisen", DisplayOrder = 3 },
                    new SurveyOption { Label = "Solo Leveling", Value = "solo-leveling", DisplayOrder = 4 }
                ]
            },
            new SurveyQuestion
            {
                Prompt = "Qual tipo de produto voce compraria primeiro?",
                HelpText = "Ajudamos a decidir o primeiro lote de producao.",
                Type = SurveyQuestionType.MultipleChoice,
                DisplayOrder = 2,
                Options =
                [
                    new SurveyOption { Label = "Canecas", Value = "canecas", DisplayOrder = 1 },
                    new SurveyOption { Label = "Quadros", Value = "quadros", DisplayOrder = 2 },
                    new SurveyOption { Label = "Kits premium", Value = "kits-premium", DisplayOrder = 3 },
                    new SurveyOption { Label = "Presentes personalizados", Value = "presentes", DisplayOrder = 4 }
                ]
            },
            new SurveyQuestion
            {
                Prompt = "Qual faixa de preco parece justa para um item premium licenciado visualmente?",
                HelpText = "Buscamos o equilibrio entre valor percebido e viabilidade.",
                Type = SurveyQuestionType.PriceRange,
                DisplayOrder = 3,
                Options =
                [
                    new SurveyOption { Label = "Ate R$ 69", Value = "ate-69", DisplayOrder = 1 },
                    new SurveyOption { Label = "R$ 70 a R$ 119", Value = "70-119", DisplayOrder = 2 },
                    new SurveyOption { Label = "R$ 120 a R$ 199", Value = "120-199", DisplayOrder = 3 },
                    new SurveyOption { Label = "Acima de R$ 200", Value = "200-plus", DisplayOrder = 4 }
                ]
            },
            new SurveyQuestion
            {
                Prompt = "Que detalhe faria voce compartilhar esta campanha com amigos?",
                HelpText = "Pode escrever livremente.",
                Type = SurveyQuestionType.Text,
                DisplayOrder = 4
            }
        ]);

        var fathersDay = new Campaign
        {
            Name = "Dia dos Pais",
            Slug = "dia-dos-pais",
            Headline = "Presentes personalizados com linguagem premium para o Dia dos Pais",
            Subheadline = "Campanha preparada para assumir o site inteiro quando chegar a hora.",
            Description = "Mantem a estrutura da plataforma e troca apenas conteudo, tema e pesquisa.",
            HeroBannerLabel = "Proxima campanha",
            Status = CampaignStatus.Scheduled,
            DisplayOrder = 2,
            IsActive = false,
            ShowPartialResults = false,
            Theme = new CampaignTheme
            {
                AccentColor = "#c9a227",
                AccentSecondaryColor = "#365d7d",
                SurfaceColor = "#10202b",
                GradientStart = "#233947",
                GradientEnd = "#0b1116",
                TextColor = "#f3efe7",
                MutedTextColor = "#a7b1b8"
            },
            Assets = [new CampaignAsset { AssetType = "hero", Label = "Banner principal", Url = "https://images.unsplash.com/photo-1519681393784-d120267933ba?auto=format&fit=crop&w=1200&q=80" }],
            Contents =
            [
                new CampaignContent { Key = "primary-cta", Value = "Quero receber o aviso" },
                new CampaignContent { Key = "secondary-cta", Value = "Entrar na Lista VIP" },
                new CampaignContent { Key = "story-title", Value = "Linha presenteavel com acabamento premium" },
                new CampaignContent { Key = "story-body", Value = "Uma proxima camada do Custom Lab para sazonalidade forte." }
            ]
        };

        var christmas = new Campaign
        {
            Name = "Natal",
            Slug = "natal",
            Headline = "Colecoes de fim de ano com atmosfera de presente premium",
            Subheadline = "Ja preparada para assumir a plataforma sem refazer rotas nem layout base.",
            Description = "O sistema inteiro foi modelado para trocar a campanha ativa por dados.",
            HeroBannerLabel = "Campanha futura",
            Status = CampaignStatus.Draft,
            DisplayOrder = 3,
            Theme = new CampaignTheme
            {
                AccentColor = "#d4af37",
                AccentSecondaryColor = "#9e2a2b",
                SurfaceColor = "#112019",
                GradientStart = "#1f4936",
                GradientEnd = "#111111",
                TextColor = "#faf5eb",
                MutedTextColor = "#d0c4b1"
            },
            Assets = [new CampaignAsset { AssetType = "hero", Label = "Banner principal", Url = "https://images.unsplash.com/photo-1482517967863-00e15c9b44be?auto=format&fit=crop&w=1200&q=80" }],
            Contents =
            [
                new CampaignContent { Key = "primary-cta", Value = "Avise-me quando entrar no ar" },
                new CampaignContent { Key = "secondary-cta", Value = "Entrar na Lista VIP" },
                new CampaignContent { Key = "story-title", Value = "Campanha de alto presenteavel para o fim do ano" },
                new CampaignContent { Key = "story-body", Value = "A mesma plataforma, outra atmosfera." }
            ]
        };

        dbContext.Campaigns.AddRange(geekCampaign, fathersDay, christmas);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
