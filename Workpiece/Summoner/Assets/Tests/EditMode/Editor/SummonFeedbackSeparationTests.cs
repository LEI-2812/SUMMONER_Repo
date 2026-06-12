using System.IO;
using NUnit.Framework;

namespace Summoner.EditModeTests
{
    public class SummonFeedbackSeparationTests
    {
        [Test]
        public void ShieldStatusEffect_DoesNotSpendTurnTime()
        {
            string text = File.ReadAllText("Assets/Script/Battle/Status/ShieldStatusEffect.cs");
            int canUpdateIndex = text.IndexOf("public bool StatusTurnCanUpdate(StatusUpdateTiming updateTiming)");
            int turnUpdateIndex = text.IndexOf("public void StatusTurnUpdate(IStatusEffectTarget target)");
            int expireIndex = text.IndexOf("public void StatusExpire(IStatusEffectTarget target)");
            string turnUpdateBody = text.Substring(turnUpdateIndex, expireIndex - turnUpdateIndex);

            Assert.GreaterOrEqual(canUpdateIndex, 0, "ShieldStatusEffect should declare turn update eligibility.");
            StringAssert.Contains("return false;", text.Substring(canUpdateIndex, turnUpdateIndex - canUpdateIndex));
            Assert.IsFalse(turnUpdateBody.Contains("effectTime--"), "Shield should not spend turn time because it expires when damage breaks it.");
        }

        [Test]
        public void AttackTargetSelection_IsSeparatedFromStatusEffect()
        {
            string targetedStrategyText = File.ReadAllText("Assets/Script/Battle/Attack/TargetedAttackStrategy.cs");
            string targetedEffectText = File.ReadAllText("Assets/Script/Battle/Attack/TargetedAttackEffectInstanceCreate.cs");

            StringAssert.Contains("SelfAttackTargetSelector", targetedStrategyText);
            StringAssert.Contains("SelectedPlateAttackTargetSelector", targetedStrategyText);
            Assert.IsFalse(targetedEffectText.Contains("target = attacker"), "Attack effects should apply to the target selected by the strategy.");
        }

        [Test]
        public void Summon_DoesNotPlayAudioSourcesDirectly()
        {
            string text = File.ReadAllText("Assets/Script/Summons/Summon.cs");

            Assert.IsFalse(text.Contains(".Play()"), "Summon should delegate sound playback to SummonSoundView.");
            StringAssert.Contains("SummonSoundView", text);
        }

        [Test]
        public void SummonSoundView_OwnsSoundPlayback()
        {
            string text = File.ReadAllText("Assets/Script/Battle/View/SummonSoundView.cs");

            StringAssert.Contains("public void AttackSoundPlay()", text);
            StringAssert.Contains("public void DebuffSoundPlay()", text);
            StringAssert.Contains("public void BuffSoundPlay()", text);
            StringAssert.Contains("audioSource.Play();", text);
        }

        [Test]
        public void Summon_DoesNotDeclareFeedbackBridgeFields()
        {
            string text = File.ReadAllText("Assets/Script/Summons/Summon.cs");

            Assert.IsFalse(text.Contains("protected Image image"), "Summon image bridge field should be removed.");
            Assert.IsFalse(text.Contains("protected Sprite[] sprites"), "Summon sprite bridge field should be removed.");
            Assert.IsFalse(text.Contains("attackSound"), "Summon sound bridge fields should be removed.");
            Assert.IsFalse(text.Contains("downHitSound"), "Summon sound bridge fields should be removed.");
            Assert.IsFalse(text.Contains("upAttackSound"), "Summon sound bridge fields should be removed.");
            Assert.IsFalse(text.Contains("ImageDataSet("), "Summon should not copy image data at runtime.");
            Assert.IsFalse(text.Contains("AudioSourcesSet("), "Summon should not copy sound data at runtime.");
        }

        [Test]
        public void Summon_DoesNotOwnActiveStatusEffectList()
        {
            string text = File.ReadAllText("Assets/Script/Summons/Summon.cs");

            Assert.IsFalse(text.Contains("activeStatusEffects"), "Summon should delegate active status storage to StatusEffectController.");
            StringAssert.Contains("statusEffectController.GetActiveStatusEffects()", text);
        }

        [Test]
        public void StatusEffectController_OwnsActiveStatusQueries()
        {
            string text = File.ReadAllText("Assets/Script/Battle/Status/StatusEffectController.cs");

            StringAssert.Contains("GetActiveStatusEffects()", text);
            StringAssert.Contains("GetStatusTypes()", text);
            StringAssert.Contains("StatusTypeContains(StatusType statusType)", text);
        }

        [Test]
        public void Summon_DelegatesImageOperationsToSummonImageView()
        {
            string text = File.ReadAllText("Assets/Script/Summons/Summon.cs");

            StringAssert.Contains("SummonImageView", text);
            StringAssert.Contains("imageView.SpriteSet(index)", text);
            StringAssert.Contains("imageView.ImageSet(image)", text);
            StringAssert.Contains("imageView.GetImage()", text);
            Assert.IsFalse(text.Contains("image.sprite = sprites[index]"), "Summon should delegate sprite changes to SummonImageView.");
        }

        [Test]
        public void SummonImageView_OwnsSpriteAndImageAccess()
        {
            string text = File.ReadAllText("Assets/Script/Battle/View/SummonImageView.cs");

            StringAssert.Contains("public void SpriteSet(int index)", text);
            StringAssert.Contains("public void ImageSet(Image image)", text);
            StringAssert.Contains("public Image GetImage()", text);
            StringAssert.Contains("image.sprite = sprites[index]", text);
        }
    }
}
