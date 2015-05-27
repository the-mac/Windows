//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXBase.h"
#include "BasicMath.h"
#include "Camera.h"
#include "Primitives.h"
#include "Collision.h"
#include "Physics.h"
#include "SDKMesh.h"
#include "UserInterface.h"
#include "Audio.h"
#include "ProfilerGraph.h"
#include "LoadScreen.h"

// Describes the constant buffer that draws the meshes.
struct ConstantBuffer
{
    float4x4 model;
    float4x4 view;
    float4x4 projection;

    float3 marblePosition;
    float marbleRadius;
    float lightStrength;
};

enum class GameState
{
    Initial,
    MainMenu,
    HighScoreDisplay,
    PreGameCountdown,
    InGameActive,
    InGamePaused,
    PostGameResults,
};

enum class CheckpointState
{
    None, // No checkpoint hit.
    Save, // A save-game checkpoint was hit.
    Goal, // The end of game "goal" point was hit.
};

namespace HybridMarbleMazeComp
{
    public delegate void DrawingSurfaceTypeToggledHandler(bool isDSBG);
    public delegate void ResolutionToggledHandler();
}

ref class MarbleMaze : public DirectXBase
{
internal:
    MarbleMaze();

    virtual void CreateDeviceResources() override;
    virtual void SetDpi(float dpi);
    virtual void CreateWindowSizeDependentResources() override;
    virtual void Render() override;
    virtual void Initialize() override;

    void LoadDeferredResources(bool delay, bool deviceOnly);

    void Update(float timeTotal, float timeDelta);
    void AddTouch(int id, Windows::Foundation::Point point);
    void UpdateTouch(int id, Windows::Foundation::Point point);
    void RemoveTouch(int id);
    Windows::Foundation::IAsyncAction^ OnSuspending();
    void OnResuming();
    void OnFocusChange(bool active);
    bool OnBackPressed();
    bool IsDSBG() { return m_isDSBG; }
    void ShowResolutionText(Platform::String^ resolutionText);

    event HybridMarbleMazeComp::DrawingSurfaceTypeToggledHandler^ DrawingSurfaceTypeToggled;
    event HybridMarbleMazeComp::ResolutionToggledHandler^ ResolutionToggled;

private:
    LoadScreen^                                         m_loadScreen;

    Microsoft::WRL::ComPtr<ID3D11InputLayout>           m_inputLayout;
    Microsoft::WRL::ComPtr<ID3D11VertexShader>          m_vertexShader;
    Microsoft::WRL::ComPtr<ID3D11PixelShader>           m_pixelShader;
    Microsoft::WRL::ComPtr<ID3D11SamplerState>          m_sampler;
    Microsoft::WRL::ComPtr<ID3D11Buffer>                m_constantBuffer;
    Microsoft::WRL::ComPtr<ID3D11BlendState>            m_blendState;
    Windows::Devices::Sensors::Accelerometer^           m_accelerometer;
    D3D_FEATURE_LEVEL                                   m_featureLevel;
    float                                               m_dpi;

    Camera^         m_camera;
    ConstantBuffer  m_mazeConstantBufferData;
    SDKMesh         m_mazeMesh;
    ConstantBuffer  m_marbleConstantBufferData;
    SDKMesh         m_marbleMesh;
    unsigned int    m_vertexStride;
    float           m_lightStrength;
    float           m_targetLightStrength;
    Collision       m_collision;
    Physics         m_physics;
    Audio           m_audio;
    GameState       m_gameState;
    bool            m_resetCamera;
    bool            m_resetMarbleRotation;
    XMFLOAT3        m_previousAcceleration;
    float           m_previousAccelerationTime;
    bool            m_loadedSave;
    bool            m_savingState;
    typedef std::vector<XMFLOAT3> Checkpoints;
    Checkpoints     m_checkpoints;
    size_t          m_currentCheckpoint;
    TextButton      m_startGameButton;
    TextButton      m_highScoreButton;
    HighScoreTable  m_highScoreTable;
    CountdownTimer  m_preGameCountdownTimer;
    StopwatchTimer  m_inGameStopwatchTimer;
    TextElement     m_checkpointText;
    TextButton      m_pausedText;
    TextElement     m_resultsText;
    HighScoreEntry  m_newHighScore;
    ProfilerGraph   m_profilerGraph;
    TextElement     m_resolutionText;
    TextElement     m_drawingSurfaceTypeText;
    ToggleCorner    m_topRightToggleCorner;
    ToggleCorner    m_topLeftToggleCorner;
    typedef std::map<int, XMFLOAT2> TouchMap;
    TouchMap        m_touches;
    typedef std::queue<Windows::Foundation::Point> PointQueue;
    PointQueue      m_pointQueue;
    bool            m_backKeyPressed;
    bool            m_windowActive;
    bool            m_deferredResourcesReady;
    bool            m_isDSBG;

    ~MarbleMaze();

    HRESULT ExtractTrianglesFromMesh(
        SDKMesh &mesh,
        const char *meshName,
        std::vector<Triangle> &triangles
        );

    void ResetCheckpoints();
    CheckpointState UpdateCheckpoints();

    void SetGameState(GameState nextState);
    Windows::Foundation::IAsyncAction^ SaveState();
    void LoadState();
};
