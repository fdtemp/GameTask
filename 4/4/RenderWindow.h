#pragma once
#include <Windows.h>
#include "4.h"
namespace RenderWindow {
	HDC hdc;
	PAINTSTRUCT ps;
	HDC memhdc;
	HBITMAP bmp;
	HPEN pen;
	HWND hwnd;
	MSG msg;
	WNDCLASS wndclass;
	bool init = false;
	Render::Renderer rend;

	LRESULT CALLBACK Wndproc(HWND hwnd, UINT message, WPARAM wparam, LPARAM lparam) {
		switch (message) {
		case WM_SIZE:
			break;
		case WM_PAINT:
			hdc = BeginPaint(hwnd, &ps);
				memhdc = CreateCompatibleDC(hdc);
				bmp = CreateCompatibleBitmap(hdc, ScreenWidth, ScreenHeight);
				SelectObject(memhdc, bmp);
					for (int i = 0;i < ScreenWidth;i++)
						for (int j = 0;j < ScreenHeight;j++) {
							Render::Color col = Render::Color(rend.Output[i][j]);
							SetPixel(memhdc, i, j, RGB(col.r, col.g, col.b));
						}
					pen = (HPEN)GetStockObject(BLACK_PEN);
					SelectObject(memhdc, pen);
					for (list<Render::Line>::iterator l = rend.LineList.begin();l != rend.LineList.end();++l) {
						MoveToEx(memhdc, floor(ScreenWidth*l->Start.x), floor(ScreenHeight*l->Start.y), NULL);
						LineTo(memhdc, floor(ScreenWidth*l->End.x), floor(ScreenHeight*l->End.y));
					}
					DeleteObject(pen);
					BitBlt(hdc, 0, 0, ScreenWidth, ScreenHeight, memhdc, 0, 0, SRCCOPY);
				DeleteObject(bmp);
				DeleteObject(memhdc);
			EndPaint(hwnd, &ps);
			break;
		case WM_DESTROY:
			PostQuitMessage(0);
			break;
		default:
			return DefWindowProc(hwnd, message, wparam, lparam);
			break;
		}
		return 0;
	}
	void WindowWaitLoop() {
		while (GetMessage(&msg, NULL, 0, 0)) {
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}
	DWORD WINAPI RefreshThread(LPVOID lpParameter) {
		RECT rect;
		rect.top = rect.left = 0;
		rect.right = ScreenWidth;
		rect.bottom = ScreenHeight;
		while (true) {
			rend.Refresh();
			InvalidateRect(hwnd, &rect, false);
			UpdateWindow(hwnd);
		}
	}

	void Init(__in HINSTANCE hInstance, __in_opt HINSTANCE hPrevInstance, __in_opt LPSTR lpCmdLine, __in int nShowCmd, Render::Renderer &renderer) {
		rend = renderer;
		
		wndclass.style = CS_HREDRAW | CS_VREDRAW;
		wndclass.lpfnWndProc = Wndproc;
		wndclass.cbClsExtra = 0;
		wndclass.cbWndExtra = 0;
		wndclass.hInstance = hInstance;
		wndclass.hIcon = LoadIcon(NULL, IDI_APPLICATION);
		wndclass.hCursor = LoadCursor(NULL, IDC_ARROW);
		wndclass.hbrBackground = (HBRUSH)GetStockObject(WHITE_BRUSH);
		wndclass.lpszMenuName = NULL;
		wndclass.lpszClassName = TEXT("Renderer");

		RegisterClass(&wndclass);
		hwnd = CreateWindow(TEXT("Renderer"), TEXT("Renderer"), WS_OVERLAPPEDWINDOW, CW_USEDEFAULT,
			CW_USEDEFAULT, ScreenWidth, ScreenHeight, NULL, NULL, hInstance, NULL);
		ShowWindow(hwnd, nShowCmd);
		CreateThread(NULL, 0, RefreshThread, NULL, 0, NULL);
	}
}