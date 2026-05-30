using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using McpUnity.Unity;

namespace McpUnity.Tools
{
    public sealed class ListAssetsTool : McpToolBase
    {
        public ListAssetsTool()
        {
            Name = "list_assets";
            Description = "Lists supported assets from the Unity AssetDatabase without modifying them";
        }

        public override JObject Execute(JObject parameters)
        {
            return AssetDatabaseReadService.ListAssets(
                parameters["folderPath"]?.ToObject<string>() ?? "Assets",
                parameters["assetType"]?.ToObject<string>(),
                parameters["limit"]?.ToObject<int>() ?? 200
            );
        }
    }

    public sealed class FindAssetsTool : McpToolBase
    {
        public FindAssetsTool()
        {
            Name = "find_assets";
            Description = "Finds supported assets in the Unity AssetDatabase without modifying them";
        }

        public override JObject Execute(JObject parameters)
        {
            return AssetDatabaseReadService.FindAssets(
                parameters["searchText"]?.ToObject<string>() ?? string.Empty,
                parameters["assetType"]?.ToObject<string>(),
                parameters["folderPaths"]?.ToObject<string[]>(),
                parameters["limit"]?.ToObject<int>() ?? 200
            );
        }
    }

    public sealed class FindReferencesByGuidTool : McpToolBase
    {
        public FindReferencesByGuidTool()
        {
            Name = "find_references_by_guid";
            Description = "Finds serialized asset files that reference a GUID without modifying them";
        }

        public override JObject Execute(JObject parameters)
        {
            return AssetDatabaseReadService.FindReferencesByGuid(
                parameters["guid"]?.ToObject<string>(),
                parameters["folderPath"]?.ToObject<string>() ?? "Assets",
                parameters["limit"]?.ToObject<int>() ?? 200
            );
        }
    }

    internal static class AssetDatabaseReadService
    {
        private static readonly Dictionary<string, string> AssetSearchTypes =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Scene"] = "Scene",
                ["Prefab"] = "Prefab",
                ["MonoScript"] = "MonoScript",
                ["ScriptableObject"] = "ScriptableObject",
                ["Material"] = "Material",
                ["Sprite"] = "Sprite"
            };

        private static readonly HashSet<string> ReferenceFileExtensions =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".unity",
                ".prefab",
                ".asset",
                ".mat"
            };

        public static JObject ListAssets(string folderPath, string assetType, int limit)
        {
            string validationError = ValidateFolderPath(folderPath);
            if (validationError != null)
            {
                return CreateValidationError(validationError);
            }

            return FindAssets(string.Empty, assetType, new[] { folderPath }, limit);
        }

        public static JObject FindAssets(string searchText, string assetType, string[] folderPaths, int limit)
        {
            string validationError = ValidateLimit(limit) ?? ValidateAssetType(assetType);
            if (validationError != null)
            {
                return CreateValidationError(validationError);
            }

            if (folderPaths != null)
            {
                foreach (string folderPath in folderPaths)
                {
                    validationError = ValidateFolderPath(folderPath);
                    if (validationError != null)
                    {
                        return CreateValidationError(validationError);
                    }
                }
            }

            string searchFilter = CreateSearchFilter(searchText, assetType);
            string[] assetGuids = folderPaths == null || folderPaths.Length == 0
                ? AssetDatabase.FindAssets(searchFilter)
                : AssetDatabase.FindAssets(searchFilter, folderPaths);

            JArray assets = new JArray();
            foreach (string guid in assetGuids.Distinct())
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetDatabase.IsValidFolder(assetPath))
                {
                    continue;
                }

                string detectedAssetType = string.IsNullOrEmpty(assetType)
                    ? DetectAssetType(assetPath)
                    : assetType;
                if (detectedAssetType == null)
                {
                    continue;
                }

                assets.Add(CreateAssetInfo(guid, assetPath, detectedAssetType));
                if (assets.Count >= limit)
                {
                    break;
                }
            }

            return new JObject
            {
                ["success"] = true,
                ["message"] = $"Retrieved {assets.Count} assets",
                ["assets"] = assets
            };
        }

        public static JObject FindReferencesByGuid(string guid, string folderPath, int limit)
        {
            string validationError = ValidateGuid(guid) ?? ValidateFolderPath(folderPath) ?? ValidateLimit(limit);
            if (validationError != null)
            {
                return CreateValidationError(validationError);
            }

            JArray references = new JArray();
            foreach (string assetGuid in AssetDatabase.FindAssets(string.Empty, new[] { folderPath }).Distinct())
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                if (!ReferenceFileExtensions.Contains(Path.GetExtension(assetPath)))
                {
                    continue;
                }

                string fullPath = GetFullAssetPath(assetPath);
                if (!File.Exists(fullPath))
                {
                    continue;
                }

                string fileContents = File.ReadAllText(fullPath);
                if (fileContents.IndexOf(guid, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                references.Add(CreateAssetInfo(assetGuid, assetPath, DetectAssetType(assetPath) ?? "SerializedAsset"));
                if (references.Count >= limit)
                {
                    break;
                }
            }

            return new JObject
            {
                ["success"] = true,
                ["message"] = $"Retrieved {references.Count} references",
                ["guid"] = guid,
                ["references"] = references
            };
        }

        private static string CreateSearchFilter(string searchText, string assetType)
        {
            string typeFilter = string.IsNullOrEmpty(assetType) ? string.Empty : $"t:{AssetSearchTypes[assetType]}";
            return string.Join(" ", new[] { searchText, typeFilter }.Where(value => !string.IsNullOrWhiteSpace(value)));
        }

        private static JObject CreateAssetInfo(string guid, string assetPath, string assetType)
        {
            return new JObject
            {
                ["name"] = Path.GetFileNameWithoutExtension(assetPath),
                ["filename"] = Path.GetFileName(assetPath),
                ["path"] = assetPath,
                ["type"] = assetType,
                ["extension"] = Path.GetExtension(assetPath).TrimStart('.'),
                ["guid"] = guid
            };
        }

        private static string DetectAssetType(string assetPath)
        {
            if (assetPath.EndsWith(".unity", StringComparison.OrdinalIgnoreCase))
            {
                return "Scene";
            }

            Type assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            if (assetType == typeof(GameObject) && assetPath.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
            {
                return "Prefab";
            }

            if (assetType == typeof(MonoScript))
            {
                return "MonoScript";
            }

            if (assetType != null && typeof(ScriptableObject).IsAssignableFrom(assetType))
            {
                return "ScriptableObject";
            }

            if (assetType == typeof(Material))
            {
                return "Material";
            }

            return IsSpriteCandidate(assetPath) &&
                   AssetDatabase.LoadAssetAtPath<Sprite>(assetPath) != null
                ? "Sprite"
                : null;
        }

        private static bool IsSpriteCandidate(string assetPath)
        {
            string extension = Path.GetExtension(assetPath);
            return extension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".psd", StringComparison.OrdinalIgnoreCase);
        }

        private static string ValidateAssetType(string assetType)
        {
            return !string.IsNullOrEmpty(assetType) && !AssetSearchTypes.ContainsKey(assetType)
                ? $"Unsupported assetType '{assetType}'. Supported values: {string.Join(", ", AssetSearchTypes.Keys)}"
                : null;
        }

        private static string ValidateFolderPath(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                return "folderPath must not be empty";
            }

            return AssetDatabase.IsValidFolder(folderPath)
                ? null
                : $"AssetDatabase folder '{folderPath}' was not found";
        }

        private static string ValidateGuid(string guid)
        {
            return !string.IsNullOrEmpty(guid) &&
                   guid.Length == 32 &&
                   guid.All(Uri.IsHexDigit)
                ? null
                : "guid must contain exactly 32 hexadecimal characters";
        }

        private static string ValidateLimit(int limit)
        {
            return limit >= 1 && limit <= 1000
                ? null
                : "limit must be between 1 and 1000";
        }

        private static JObject CreateValidationError(string message)
        {
            return McpUnitySocketHandler.CreateErrorResponse(message, "validation_error");
        }

        private static string GetFullAssetPath(string assetPath)
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, "..", assetPath));
        }
    }
}
