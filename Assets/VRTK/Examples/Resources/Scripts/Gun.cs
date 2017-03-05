namespace VRTK.Examples
{
    using UnityEngine;

    public class Gun : ItemMachine
    {
        private GameObject bullet;
        private float bulletSpeed = 1000f;
        private float bulletLife = 5f;

        public override void StartUsing(GameObject usingObject)
        {
            base.StartUsing(usingObject);
            FireBullet();
        }

        protected void Start()
        {
            bullet = transform.Find("Bullet").gameObject;
            bullet.SetActive(false);
        }

        private void FireBullet()
        {
            ItemMachine im = PlayerMachine.instance.CreateItem(bullet, bullet.transform.position, bullet.transform.rotation, false, null,"", true);
            im.gameObject.SetActive(true);
            Rigidbody rb = im.GetComponent<Rigidbody>();
            rb.AddForce(-bullet.transform.forward * bulletSpeed);
            Destroy(im.gameObject, bulletLife);
        }
    }
}