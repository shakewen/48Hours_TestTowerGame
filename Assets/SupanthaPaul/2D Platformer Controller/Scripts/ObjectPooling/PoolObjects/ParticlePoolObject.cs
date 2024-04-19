using UnityEngine;

namespace SupanthaPaul
{
	//继承PoolObject，PoolObject中有一个空的抽象函数，
	public class ParticlePoolObject : PoolObject
	{
		ParticleSystem _particle;

		//重写粒子特效再次使用
		public override void OnObjectReuse()
		{
			if(!_particle)
				_particle = GetComponent<ParticleSystem>();
			_particle.Play();
		}
	}
}
