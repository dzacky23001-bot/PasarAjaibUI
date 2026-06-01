#if UNITY_EDITOR
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEditor;
using UnityEngine;
using McpUnity.Unity;

/// <summary>
/// Solusi permanen: MCP server otomatis restart setelah domain reload
/// tanpa perlu restart Unity maupun Claude Code.
///
/// Cara kerja:
/// - Setiap domain reload selesai, cek apakah server sudah jalan
/// - Kalau belum, cari port bebas (8095–8105) dan start server
/// - Update McpUnitySettings.json agar node.js bridge bisa reconnect ke port baru
/// </summary>
[InitializeOnLoad]
public static class McpPortFix
{
    static readonly string SETTINGS_PATH = Path.Combine(
        Directory.GetCurrentDirectory(), "ProjectSettings", "McpUnitySettings.json");

    const int BASE_PORT  = 8095;
    const int MAX_PORT   = 8105;
    const double DELAY   = 1.0;   // detik setelah domain reload sebelum coba start
    const int MAX_RETRY  = 10;

    static double _nextCheck = -1;
    static int    _retries   = 0;

    static McpPortFix()
    {
        EditorApplication.update += Tick;
        // Jadwalkan pengecekan 1 detik setelah domain reload selesai
        _nextCheck = EditorApplication.timeSinceStartup + DELAY;
        _retries   = 0;
    }

    static void Tick()
    {
        if (_nextCheck < 0 || EditorApplication.timeSinceStartup < _nextCheck) return;
        _nextCheck = -1;

        var server = GetServer();
        if (server == null || server.IsListening)
        {
            // Server sudah jalan atau tidak perlu cek lagi
            Unsubscribe();
            return;
        }

        _retries++;
        if (_retries > MAX_RETRY)
        {
            Debug.LogWarning("[McpPortFix] Melebihi batas retry. Gunakan: PasarAjaib > Restart MCP Server");
            Unsubscribe();
            return;
        }

        int port = FindFreePort();
        if (port < 0)
        {
            Debug.LogWarning($"[McpPortFix] Semua port sibuk, retry {_retries}/{MAX_RETRY}...");
            _nextCheck = EditorApplication.timeSinceStartup + DELAY;
            return;
        }

        WritePort(port);
        server.StartServer();

        if (server.IsListening)
        {
            Debug.Log($"[McpPortFix] ✅ MCP Server aktif di port {port}");
            Unsubscribe();
        }
        else
        {
            Debug.LogWarning($"[McpPortFix] StartServer gagal (retry {_retries}), coba lagi...");
            _nextCheck = EditorApplication.timeSinceStartup + DELAY;
        }
    }

    [MenuItem("PasarAjaib/Restart MCP Server")]
    public static void RestartMcpServer()
    {
        var server = GetServer();
        if (server == null) { Debug.LogError("[McpPortFix] Instance tidak ditemukan."); return; }

        if (server.IsListening) server.StopServer();
        System.Threading.Thread.Sleep(300); // beri waktu port release

        int port = FindFreePort();
        if (port < 0) { Debug.LogError("[McpPortFix] Tidak ada port bebas!"); return; }

        WritePort(port);
        server.StartServer();

        Debug.Log(server.IsListening
            ? $"[McpPortFix] ✅ Restart sukses di port {port}"
            : "[McpPortFix] ❌ Gagal. Tutup Unity instance lain jika ada.");
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    static int FindFreePort()
    {
        for (int p = BASE_PORT; p <= MAX_PORT; p++)
        {
            try
            {
                using var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.ExclusiveAddressUse = false;
                sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                sock.Bind(new IPEndPoint(IPAddress.Any, p));
                return p; // port bebas
            }
            catch { /* port sibuk, coba berikutnya */ }
        }
        return -1;
    }

    static void WritePort(int port)
    {
        try
        {
            if (!File.Exists(SETTINGS_PATH)) return;
            string json = File.ReadAllText(SETTINGS_PATH);

            int idx = json.IndexOf("\"Port\"", StringComparison.OrdinalIgnoreCase);
            if (idx < 0) return;
            int colon = json.IndexOf(':', idx);
            if (colon < 0) return;
            int s = colon + 1;
            while (s < json.Length && (json[s] == ' ' || json[s] == '\t')) s++;
            int e = s;
            while (e < json.Length && char.IsDigit(json[e])) e++;
            int old = int.Parse(json.Substring(s, e - s));
            if (old == port) return;

            File.WriteAllText(SETTINGS_PATH, json.Substring(0, s) + port + json.Substring(e));
            Debug.Log($"[McpPortFix] Port diupdate: {old} → {port}");
        }
        catch (Exception ex) { Debug.LogWarning($"[McpPortFix] WritePort error: {ex.Message}"); }
    }

    static McpUnityServer GetServer()
    {
        try { return McpUnityServer.Instance; } catch { return null; }
    }

    static void Unsubscribe()
    {
        EditorApplication.update -= Tick;
    }
}
#endif
