using UnityEngine;

using Zenject;

using HereticalSolutions.Quests.Factories;

namespace HereticalSolutions.Quests.DI
{
    public class QuestInstaller : MonoInstaller
    {
        [SerializeField]
        private QuestsSettings questsSettings;
        
        public override void InstallBindings()
        {
            var activeQuestObjectivesManager = QuestsFactory.BuildActiveQuestObjectivesManager();

            if (!Container.HasBinding<ActiveQuestObjectivesManager>())
                Container.Bind<ActiveQuestObjectivesManager>().FromInstance(activeQuestObjectivesManager).AsCached();
            
            var activeQuestManager = QuestsFactoryUnity.BuildActiveQuestManager(questsSettings, activeQuestObjectivesManager);

            if (!Container.HasBinding<ActiveQuestsManager>())
                Container.Bind<ActiveQuestsManager>().FromInstance(activeQuestManager).AsCached();
        }
    }
}