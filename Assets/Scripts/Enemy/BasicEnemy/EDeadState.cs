namespace wine.enemy
{
    public class EDeadState : EBaseState 
    {
        public EDeadState(EnemyBaseController e, EnemyData data) : base(e, data) {}

        public override void Enter()
        {
            // PlayerStats.instances.AddExperiences(data.expValue);
            // PlayerStats.instances.ControlVessel(Random.Range(data.bloodCountMin, data.bloodCountMax));
            e.gameObject.SetActive(false);
        }
    }
}
