using System.Collections.Generic;
using System;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Summoner.EditModeTests
{
    public class SummonStatusViewTests
    {
        private GameObject testObject;
        private Image image;
        private Component view;
        private Type statusTypeType;
        private Type statusEffectType;

        [SetUp]
        public void SetUp()
        {
            testObject = new GameObject("SummonStatusView Test");
            image = testObject.AddComponent<Image>();
            view = testObject.AddComponent(GetTypeByName("SummonStatusView"));
            statusTypeType = GetTypeByName("StatusType");
            statusEffectType = GetTypeByName("StatusEffect");
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(testObject);
        }

        [TestCase("Burn", 1f, 0.35f, 0.15f)]
        [TestCase("Poison", 0.3f, 1f, 0.3f)]
        [TestCase("Stun", 0.2f, 0.2f, 0.2f)]
        [TestCase("Curse", 0.64f, 0.19f, 0.84f)]
        [TestCase("LifeDrain", 1f, 1f, 0.5f)]
        [TestCase("Shield", 0.35f, 0.75f, 1f)]
        [TestCase("Upgrade", 0.35f, 0.55f, 1f)]
        [TestCase("OnceInvincibility", 1f, 0.85f, 0.25f)]
        [TestCase("Heal", 0.2f, 0.85f, 0.35f)]
        public void StatusEffectsShow_SetsColorByStatus(string statusTypeName, float expectedR, float expectedG, float expectedB)
        {
            StatusEffectsShow(StatusEffectList(statusTypeName), false);

            AssertColor(expectedR, expectedG, expectedB);
        }

        [Test]
        public void StatusEffectsShow_ResetsColor_WhenNoStatusExists()
        {
            StatusEffectsShow(StatusEffectList("Poison"), false);

            StatusEffectsShow(StatusEffectList(), false);

            Assert.AreEqual(Color.white, image.color);
        }

        [Test]
        public void StatusEffectsShow_MultipleStatuses_ShowsFirstStatusImmediately()
        {
            StatusEffectsShow(StatusEffectList("Poison", "Stun"), false);

            AssertColor(0.3f, 1f, 0.3f);
        }

        private void StatusEffectsShow(object effects, bool isPaused)
        {
            Type effectsParameterType = typeof(IReadOnlyList<>).MakeGenericType(statusEffectType);
            MethodInfo method = view.GetType().GetMethod(
                "StatusEffectsShow",
                new[] { effectsParameterType, typeof(bool) });

            Assert.IsNotNull(method, "SummonStatusView.StatusEffectsShow(IReadOnlyList<StatusEffect>, bool) was not found.");
            method.Invoke(view, new[] { effects, isPaused });
        }

        private object StatusEffectList(params string[] statusTypeNames)
        {
            Type listType = typeof(List<>).MakeGenericType(statusEffectType);
            var effects = (System.Collections.IList)Activator.CreateInstance(listType);

            foreach (string statusTypeName in statusTypeNames)
            {
                object statusType = Enum.Parse(statusTypeType, statusTypeName);
                effects.Add(Activator.CreateInstance(statusEffectType, statusType, 1, 0d, null));
            }

            return effects;
        }

        private static Type GetTypeByName(string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            Assert.Fail(typeName + " type was not found.");
            return null;
        }

        private void AssertColor(float expectedR, float expectedG, float expectedB)
        {
            Assert.AreEqual(expectedR, image.color.r, 0.001f);
            Assert.AreEqual(expectedG, image.color.g, 0.001f);
            Assert.AreEqual(expectedB, image.color.b, 0.001f);
            Assert.AreEqual(1f, image.color.a, 0.001f);
        }
    }
}
