using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VContainer;
using VContainer.Unity;
using UnityEngine.SceneManagement;

namespace PSB.Architect
{
    public sealed class ScoreLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // �e�Ƃ���RootLifeTimeScope���ݒ肳��Ă���̂�
            // ����LifetimeScope�ɓo�^�����I�u�W�F�N�g�ɑ΂��Ă����������B
        }
    }
}