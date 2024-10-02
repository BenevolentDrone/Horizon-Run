using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Quests.Factories
{
    public static class QuestsFactoryUnity
    {
        public static ActiveQuestsManager BuildActiveQuestManager(
            QuestsSettings questsSettings,
            ActiveQuestObjectivesManager activeQuestObjectivesManager)
        {
            return new ActiveQuestsManager(
                BuildQuestDatabase(questsSettings),
                QuestsFactory.BuildPrototypeStages(),
                RepositoriesFactory.BuildDictionaryRepository<string, ActiveQuest>(),
                activeQuestObjectivesManager);
        }

        public static IReadOnlyRepository<string, Quest> BuildQuestDatabase(QuestsSettings questsSettings)
        {
            IRepository<string, Quest> database = RepositoriesFactory.BuildDictionaryRepository<string, Quest>();

            for (int i = 0; i < questsSettings.Quests.Length; i++)
            {
                var questDescriptor = questsSettings.Quests[i];

                var quest = QuestsFactory.BuildQuest(questDescriptor, EQuestStatus.IDLE);

                database.Add(questDescriptor.ID, quest);
            }

            return (IReadOnlyRepository<string, Quest>)database;
        }
    }
}