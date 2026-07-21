namespace BestVipCustomLab.Domain;

public enum CampaignStatus
{
    Draft = 0,
    Scheduled = 1,
    Active = 2,
    Archived = 3
}

public enum SurveyQuestionType
{
    SingleChoice = 0,
    MultipleChoice = 1,
    Text = 2,
    PriceRange = 3
}

public enum InteractionType
{
    Contact = 0,
    SocialShare = 1,
    PageVisit = 2
}
