#pragma once
#include <Windows.h>
#include <bitset>
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
	byte mbmp[ScreenWidth * ScreenHeight * 3];
	BITMAPINFO bmpinfo;
	int c = 0;

	LRESULT CALLBACK Wndproc(HWND hwnd, UINT message, WPARAM wparam, LPARAM lparam) {
		switch (message) {
		case WM_SIZE:
			break;
		case WM_PAINT:
			hdc = BeginPaint(hwnd, &ps);
				memhdc = CreateCompatibleDC(hdc);
				bmp = CreateCompatibleBitmap(hdc, ScreenWidth, ScreenHeight);
				SelectObject(memhdc, bmp);
				/*fh.bfType = 0x4D42;
				fh.bfSize = 54 + ScreenWidth * ScreenHeight * 4;
				fh.bfReserved1 = 0;
				fh.bfReserved2 = 0;
				fh.bfOffBits = 54;*/
				bmpinfo.bmiHeader.biSize = 40;
				bmpinfo.bmiHeader.biWidth = ScreenWidth;
				bmpinfo.bmiHeader.biHeight = ScreenHeight;
				bmpinfo.bmiHeader.biPlanes = 1;
				bmpinfo.bmiHeader.biBitCount = 24;
				bmpinfo.bmiHeader.biCompression = BI_RGB;
				bmpinfo.bmiHeader.biSizeImage = 0;
				bmpinfo.bmiHeader.biXPelsPerMeter = 0;
				bmpinfo.bmiHeader.biYPelsPerMeter = 0;
				bmpinfo.bmiHeader.biClrUsed = 256 * 256 * 256;
				bmpinfo.bmiHeader.biClrImportant = 0;

					for (int i = 0;i < ScreenWidth;i++)
						for (int j = 0;j < ScreenHeight;j++) {
							Render::Color col = Render::Color(rend.Output[i][j]);
							//SetPixel(memhdc, i, j, RGB(col.r, col.g, col.b));
							mbmp[3 * ((ScreenWidth - i - 1) + (ScreenHeight - j - 1) * ScreenWidth) + 2] = col.r;
							mbmp[3 * ((ScreenWidth - i - 1) + (ScreenHeight - j - 1) * ScreenWidth) + 1] = col.g;
							mbmp[3 * ((ScreenWidth - i - 1) + (ScreenHeight - j - 1) * ScreenWidth) + 0] = col.b;
						}
					if (!(c = SetDIBitsToDevice(memhdc, 0, 0, ScreenWidth, ScreenHeight, 0, 0, 0, ScreenHeight, &mbmp, &bmpinfo, DIB_RGB_COLORS)))
						cout << "Set DIB error !!!" << endl;
					else
						cout << c << endl;
					pen = (HPEN)GetStockObject(BLACK_PEN);
					SelectObject(memhdc, pen);
					for (list<Render::ScreenLine>::iterator l = rend.LineList.begin();l != rend.LineList.end();++l) {
						MoveToEx(memhdc, l->Start.x, l->Start.y, NULL);
						LineTo(memhdc, l->End.x, l->End.y);
					}
					DeleteObject(pen);
					BitBlt(hdc, 0, 0, ScreenWidth, ScreenHeight, memhdc, 0, 0, SRCCOPY);
				cout << "----------info" << endl;
				cout << bmpinfo.bmiHeader.biSize << endl;
				cout << bmpinfo.bmiHeader.biWidth << endl;
				cout << bmpinfo.bmiHeader.biHeight << endl;
				cout << bmpinfo.bmiHeader.biPlanes << endl;
				cout << bmpinfo.bmiHeader.biBitCount << endl;
				cout << bmpinfo.bmiHeader.biCompression << endl;
				cout << bmpinfo.bmiHeader.biSizeImage << endl;
				cout << bmpinfo.bmiHeader.biXPelsPerMeter << endl;
				cout << bmpinfo.bmiHeader.biYPelsPerMeter << endl;
				cout << bmpinfo.bmiHeader.biClrUsed << endl;
				cout << bmpinfo.bmiHeader.biClrImportant << endl;

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