import { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import { CallToolResult } from "@modelcontextprotocol/sdk/types.js";
import * as z from "zod";
import { McpUnity } from "../unity/mcpUnity.js";
import { McpUnityError, ErrorType } from "../utils/errors.js";
import { Logger } from "../utils/logger.js";

const assetTypeSchema = z
  .enum(["Scene", "Prefab", "MonoScript", "ScriptableObject", "Material", "Sprite"])
  .optional()
  .describe("Optional supported Unity asset type");

const limitSchema = z
  .number()
  .int()
  .min(1)
  .max(1000)
  .optional()
  .describe("Maximum number of results to return. Defaults to 200.");

export function registerListAssetsTool(server: McpServer, mcpUnity: McpUnity, logger: Logger) {
  registerAssetDatabaseReadTool(
    server,
    mcpUnity,
    logger,
    "list_assets",
    "Lists supported assets in an AssetDatabase folder without modifying them",
    z.object({
      folderPath: z.string().optional().describe("AssetDatabase folder path. Defaults to Assets."),
      assetType: assetTypeSchema,
      limit: limitSchema,
    })
  );
}

export function registerFindAssetsTool(server: McpServer, mcpUnity: McpUnity, logger: Logger) {
  registerAssetDatabaseReadTool(
    server,
    mcpUnity,
    logger,
    "find_assets",
    "Finds supported assets with AssetDatabase search without modifying them",
    z.object({
      searchText: z.string().optional().describe("Optional AssetDatabase search text"),
      assetType: assetTypeSchema,
      folderPaths: z.array(z.string()).optional().describe("Optional AssetDatabase folders to search"),
      limit: limitSchema,
    })
  );
}

export function registerFindReferencesByGuidTool(server: McpServer, mcpUnity: McpUnity, logger: Logger) {
  registerAssetDatabaseReadTool(
    server,
    mcpUnity,
    logger,
    "find_references_by_guid",
    "Finds serialized asset files that reference a GUID without modifying them",
    z.object({
      guid: z.string().regex(/^[0-9a-fA-F]{32}$/).describe("Asset GUID to search for"),
      folderPath: z.string().optional().describe("AssetDatabase folder path. Defaults to Assets."),
      limit: limitSchema,
    })
  );
}

function registerAssetDatabaseReadTool(
  server: McpServer,
  mcpUnity: McpUnity,
  logger: Logger,
  toolName: string,
  toolDescription: string,
  paramsSchema: z.ZodObject<any>
) {
  logger.info(`Registering tool: ${toolName}`);

  server.tool(
    toolName,
    toolDescription,
    paramsSchema.shape,
    async (params: any) => {
      try {
        logger.info(`Executing tool: ${toolName}`, params);
        const response = await mcpUnity.sendRequest({ method: toolName, params });
        if (!response.success) {
          throw new McpUnityError(
            ErrorType.TOOL_EXECUTION,
            response.message || `Failed to execute ${toolName}`
          );
        }

        logger.info(`Tool execution successful: ${toolName}`);
        return createAssetDatabaseReadResult(response);
      } catch (error) {
        logger.error(`Tool execution failed: ${toolName}`, error);
        throw error;
      }
    }
  );
}

function createAssetDatabaseReadResult(response: any): CallToolResult {
  return {
    content: [
      {
        type: "text",
        text: JSON.stringify(response, null, 2),
      },
    ],
  };
}
