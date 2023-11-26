namespace Inkapsulation
{
    public interface IDamagable
    {
        void TakeDamage(int damage);
    }

    public interface IShootable
    {
        public event EventHandler OutOfAmmo;
        void Fire(IDamagable target);
        void Reload();
    }
    
    public class Weapon : IShootable
    {
        private readonly int _damage;
        private readonly int _maxMagazineBullets;
        private int _bulletsInMagazine;

        public Weapon(int damage, int maxMagazineBullets)
        {
            if (damage <= 0)
                throw new ArgumentException("Damage must be more than 0", nameof(damage));

            if (maxMagazineBullets <= 0)
                throw new ArgumentException("Magazine must be more than 0", nameof(maxMagazineBullets));
            
            _damage = damage;
            _maxMagazineBullets = maxMagazineBullets;
            _bulletsInMagazine = _maxMagazineBullets;
        }

        public event EventHandler? OutOfAmmo;

        public void Fire(IDamagable target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            
            if (_bulletsInMagazine == 0)
            {
                OutOfAmmo?.Invoke(this, EventArgs.Empty);
                return;
            }

            target.TakeDamage(_damage);
            _bulletsInMagazine--;
        }

        public void Reload()
        {
            _bulletsInMagazine = _maxMagazineBullets;
        }
    }

    public class Player : IDamagable
    {
        private int _health;

        public Player(int health)
        {
            if (_health <= 0)
                throw new ArgumentException(nameof(_health));
            
            _health = health;
        }

        public void TakeDamage(int damage)
        {
            if (damage < 0)
                throw new ArgumentException("Damage must be more or equals than 0", nameof(damage));
            
            _health -= damage;
        }
    }

    public class Bot
    {
        private readonly IShootable _weapon;

        public Bot(IShootable weapon)
        {
            if (_weapon == null)
                throw new ArgumentNullException(nameof(weapon));
            
            _weapon = weapon;
            _weapon.OutOfAmmo += OnOutOfAmmo;
        }

        private void OnOutOfAmmo(object? sender, EventArgs e)
        {
            _weapon.Reload();
        }

        public void OnSeePlayer(IDamagable target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            
            _weapon.Fire(target);
        }
    }
}