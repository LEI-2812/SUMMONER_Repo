using System.IO;
using NUnit.Framework;

namespace Summoner.EditModeTests
{
    public class SummonFeedbackSeparationTests
    {
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
            StringAssert.Contains("statusEffectController.ActiveStatusEffectsGet()", text);
        }

        [Test]
        public void StatusEffectController_OwnsActiveStatusQueries()
        {
            string text = File.ReadAllText("Assets/Script/Battle/Status/StatusEffectController.cs");

            StringAssert.Contains("ActiveStatusEffectsGet()", text);
            StringAssert.Contains("StatusTypesGet()", text);
            StringAssert.Contains("StatusTypeContains(StatusType statusType)", text);
        }

        [Test]
        public void Summon_DelegatesImageOperationsToSummonImageView()
        {
            string text = File.ReadAllText("Assets/Script/Summons/Summon.cs");

            StringAssert.Contains("SummonImageView", text);
            StringAssert.Contains("imageView.SpriteSet(index)", text);
            StringAssert.Contains("imageView.ImageSet(image)", text);
            StringAssert.Contains("imageView.ImageGet()", text);
            Assert.IsFalse(text.Contains("image.sprite = sprites[index]"), "Summon should delegate sprite changes to SummonImageView.");
        }

        [Test]
        public void SummonImageView_OwnsSpriteAndImageAccess()
        {
            string text = File.ReadAllText("Assets/Script/Battle/View/SummonImageView.cs");

            StringAssert.Contains("public void SpriteSet(int index)", text);
            StringAssert.Contains("public void ImageSet(Image image)", text);
            StringAssert.Contains("public Image ImageGet()", text);
            StringAssert.Contains("image.sprite = sprites[index]", text);
        }
    }
}
