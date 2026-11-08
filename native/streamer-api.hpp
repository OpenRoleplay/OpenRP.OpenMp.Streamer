// Mirror of samp-streamer-plugin/src/streamer_component_api.h.
// Keep in sync with the streamer fork; defines the ABI between streamer.dll and this component.

#pragma once

#include <component.hpp>
#include <cstdint>

constexpr UID kStreamerComponentUID = UID(0x53744d72506c674eULL);      // "StMrPlgN"
constexpr UID kStreamerExtensionUID = UID(0x53744d72506c6758ULL);      // "StMrPlgX"

enum class StreamerHandlerResult : int
{
    Continue = 0,
    Consume  = 1,
    Veto     = 2,
};

struct IStreamerEventHandler
{
    virtual ~IStreamerEventHandler() = default;

    virtual void onPlayerPickUpDynamicPickup(int playerId, int pickupId) {}
    virtual void onPlayerEnterDynamicCheckpoint(int playerId, int cpId) {}
    virtual void onPlayerLeaveDynamicCheckpoint(int playerId, int cpId) {}
    virtual void onPlayerEnterDynamicRaceCheckpoint(int playerId, int cpId) {}
    virtual void onPlayerLeaveDynamicRaceCheckpoint(int playerId, int cpId) {}
    virtual void onPlayerEnterDynamicArea(int playerId, int areaId) {}
    virtual void onPlayerLeaveDynamicArea(int playerId, int areaId) {}
    virtual void onDynamicObjectMoved(int objectId) {}
    virtual void onDynamicObjectStreamIn(int objectId, int forPlayerId) {}
    virtual void onDynamicObjectStreamOut(int objectId, int forPlayerId) {}
    virtual void onDynamicPickupStreamIn(int pickupId, int forPlayerId) {}
    virtual void onDynamicPickupStreamOut(int pickupId, int forPlayerId) {}
    virtual void onDynamicTextLabelStreamIn(int labelId, int forPlayerId) {}
    virtual void onDynamicTextLabelStreamOut(int labelId, int forPlayerId) {}
    virtual void onDynamicCheckpointStreamIn(int cpId, int forPlayerId) {}
    virtual void onDynamicCheckpointStreamOut(int cpId, int forPlayerId) {}
    virtual void onDynamicMapIconStreamIn(int iconId, int forPlayerId) {}
    virtual void onDynamicMapIconStreamOut(int iconId, int forPlayerId) {}

    virtual StreamerHandlerResult onPlayerEditDynamicObject(int, int, int,
        float, float, float, float, float, float) { return StreamerHandlerResult::Continue; }
    virtual StreamerHandlerResult onPlayerSelectDynamicObject(int, int, int,
        float, float, float) { return StreamerHandlerResult::Continue; }
    virtual StreamerHandlerResult onPlayerShootDynamicObject(int, int, int,
        float, float, float) { return StreamerHandlerResult::Continue; }
};

struct IStreamerComponent : public IExtension
{
    PROVIDE_EXT_UID(kStreamerExtensionUID)

    virtual int  createObject(int modelId, float posX, float posY, float posZ,
        float rotX, float rotY, float rotZ,
        int worldId, int interiorId, int playerId,
        float streamDistance, float drawDistance,
        int areaId, int priority) = 0;
    virtual bool destroyObject(int objectId) = 0;
    virtual bool isValidObject(int objectId) = 0;

    virtual int  moveObject(int objectId,
        float targetX, float targetY, float targetZ,
        float speed, float rotX, float rotY, float rotZ) = 0;
    virtual bool stopObject(int objectId) = 0;
    virtual bool isObjectMoving(int objectId) = 0;

    virtual bool getObjectPos(int objectId, float& outX, float& outY, float& outZ) = 0;
    virtual bool setObjectPos(int objectId, float x, float y, float z) = 0;
    virtual bool getObjectRot(int objectId, float& outX, float& outY, float& outZ) = 0;
    virtual bool setObjectRot(int objectId, float x, float y, float z) = 0;

    virtual bool attachObjectToObject(int objectId, int parentObjectId,
        float offX, float offY, float offZ,
        float rotX, float rotY, float rotZ,
        bool syncRotation) = 0;
    virtual bool attachObjectToPlayer(int objectId, int playerId,
        float offX, float offY, float offZ,
        float rotX, float rotY, float rotZ) = 0;
    virtual bool attachObjectToVehicle(int objectId, int vehicleId,
        float offX, float offY, float offZ,
        float rotX, float rotY, float rotZ) = 0;

    virtual bool setObjectMaterial(int objectId, int materialIndex,
        int modelId, const char* txdName, const char* textureName,
        uint32_t materialColor) = 0;
    virtual bool setObjectMaterialText(int objectId, int materialIndex,
        const char* text, int materialSize, const char* fontFace,
        int fontSize, bool bold, uint32_t fontColor, uint32_t backColor,
        int alignment) = 0;

    virtual bool editObject(int playerId, int objectId) = 0;

    virtual int  createPickup(int modelId, int type,
        float posX, float posY, float posZ,
        int worldId, int interiorId, int playerId,
        float streamDistance, int areaId, int priority) = 0;
    virtual bool destroyPickup(int pickupId) = 0;
    virtual bool isValidPickup(int pickupId) = 0;

    virtual int  createTextLabel(const char* text, uint32_t color,
        float posX, float posY, float posZ, float drawDistance,
        int attachedPlayer, int attachedVehicle, bool testLos,
        int worldId, int interiorId, int playerId,
        float streamDistance, int areaId, int priority) = 0;
    virtual bool destroyTextLabel(int labelId) = 0;
    virtual bool updateTextLabelText(int labelId, uint32_t color, const char* text) = 0;
    virtual bool isValidTextLabel(int labelId) = 0;

    virtual int  createMapIcon(float posX, float posY, float posZ,
        int type, uint32_t color,
        int worldId, int interiorId, int playerId,
        float streamDistance, int style, int areaId, int priority) = 0;
    virtual bool destroyMapIcon(int iconId) = 0;
    virtual bool isValidMapIcon(int iconId) = 0;

    virtual int  createCheckpoint(float posX, float posY, float posZ, float size,
        int worldId, int interiorId, int playerId,
        float streamDistance, int areaId, int priority) = 0;
    virtual bool destroyCheckpoint(int cpId) = 0;
    virtual bool isValidCheckpoint(int cpId) = 0;

    virtual int  createActor(int modelId, float x, float y, float z, float rotation,
        bool invulnerable, float health, float streamDistance,
        int worldId, int interiorId, int playerId, int areaId, int priority) = 0;
    virtual bool destroyActor(int actorId) = 0;
    virtual bool isValidActor(int actorId) = 0;

    virtual bool applyActorAnimation(int actorId, const char* animLib, const char* animName,
        float delta, bool loop, bool lockX, bool lockY, bool freeze, int timeMs) = 0;
    virtual bool clearActorAnimations(int actorId) = 0;

    virtual bool getActorPos(int actorId, float& outX, float& outY, float& outZ) = 0;
    virtual bool setActorPos(int actorId, float x, float y, float z) = 0;
    virtual bool getActorFacingAngle(int actorId, float& outAngle) = 0;
    virtual bool setActorFacingAngle(int actorId, float angle) = 0;

    virtual bool getActorHealth(int actorId, float& outHealth) = 0;
    virtual bool setActorHealth(int actorId, float health) = 0;
    virtual bool isActorInvulnerable(int actorId) = 0;
    virtual bool setActorInvulnerable(int actorId, bool invulnerable) = 0;
    virtual int  getActorVirtualWorld(int actorId) = 0;
    virtual bool setActorVirtualWorld(int actorId, int worldId) = 0;

    // Telemetry (VS:RP fork) — per-phase timings + stream counters since last reset.
    virtual uint64_t getPhaseTimeNs(int type) = 0;
    virtual uint64_t getPhaseAvgUs(int type) = 0;
    virtual uint64_t getPhaseTickCount() = 0;
    virtual uint64_t getPhaseStreamInCount(int type) = 0;
    virtual uint64_t getPhaseStreamOutCount(int type) = 0;
    virtual void     resetPhaseStats() = 0;

    // Anti-flicker hysteresis (VS:RP fork).
    virtual float getHysteresisFactor(int type) = 0;
    virtual bool  setHysteresisFactor(int type, float value) = 0;

    // Two-tier grid (VS:RP fork).
    virtual float getCoarseCellSize() = 0;
    virtual bool  setCoarseCellSize(float size) = 0;
    virtual float getCoarseCellDistance() = 0;
    virtual bool  setCoarseCellDistance(float distance) = 0;

    virtual void addEventHandler(IStreamerEventHandler* handler) = 0;
    virtual void removeEventHandler(IStreamerEventHandler* handler) = 0;

    // Per-player streaming settings (OpenRP streamer fork, KoertLichtendonk/omp-streamer-component).
    // Appended after every other method, matching the fork's vtable. type = STREAMER_TYPE_*; playerId -1 = all players.
    virtual int   getVisibleItems(int type, int playerId) = 0;
    virtual bool  setVisibleItems(int type, int items, int playerId) = 0;
    virtual float getRadiusMultiplier(int type, int playerId) = 0;
    virtual bool  setRadiusMultiplier(int type, float multiplier, int playerId) = 0;
    virtual bool  updatePlayer(int playerId, int type) = 0;

    void reset() override {}
};
