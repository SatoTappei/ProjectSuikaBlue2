using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.Game
{
    public static class Translator
    {
        /// <summary>
        /// ���݂̃Q�[���̏�Ԃ���API�Ƀ��N�G�X�g���郁�b�Z�[�W�𐶐�
        /// </summary>
        public static string Translate(IReadOnlyGameState gameState)
        {
            string request = string.Empty;
            if (gameState.OnStageBorder) request += "����ȏ�i�ނƃX�e�[�W���痎���Ă��܂���������܂���B";
            if (gameState.OnHoleFront) request += "�ڂ̑O�ɂ̓W�����v�Ŕ�щz�����錊������܂��B";
            if (gameState.OnStepFront) request += "�ڂ̑O�ɂ̓W�����v�Ŕ�щz������i��������܂��B";
            if (!(gameState.OnStageBorder ||
                  gameState.OnHoleFront ||
                  gameState.OnStepFront)) request += "���i�ł��铹�������Ă��܂��B";

            request += "���Ȃ��͂ǂ̕����ɐi�݂܂����H";

            return request;
        }

        /// <summary>
        /// �v���C���[�����M�������e����API�Ƀ��N�G�X�g���郁�b�Z�[�W�𐶐�
        /// </summary>
        public static string Translate(string playerSend)
        {
            string request = "���̎w���ɏ]���Ă��������B" + playerSend;
            return request;
        }
    }
}