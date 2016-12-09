#include <Windows.h>
#include <iostream>
#include "4.h"
#include "Renderer.h"
#include "RenderWindow.h"


LRESULT CALLBACK RenderWindow::Wndproc(HWND, UINT, WPARAM, LPARAM);

int WINAPI WinMain(__in HINSTANCE hInstance, __in_opt HINSTANCE hPrevInstance, __in_opt LPSTR lpCmdLine, __in int nShowCmd) {

	ofstream fout("output.txt");
	streambuf *coutbackup;
	coutbackup = cout.rdbuf(fout.rdbuf());

	Render::Renderer *rend = new Render::Renderer();

	/*mat a(1, 4);
	a << 1 << 0 << 0 << 1 << endr;
	Render::Transform trans = Render::Transform(
		Render::Vector3(10, 10, 10),
		Render::AxisAngle(
			Render::Vector3(0, 0, 1),
			3.1415926f *0.5
		),
		Render::Vector3(0.5, 0.5, 0.5)
	);
	a = a * trans.GetTransformMat();
	a.print("a :");
	trans.GetTranslationMat(trans.Translation).print("Translation :");
	trans.GetRotationMat(trans.Rotation).print("Rotation :");
	trans.GetScaleMat(trans.Scale).print("Scale :");
	trans.GetTransformMat().print("Transform :");
	system("pause");*/

	RenderWindow::Init(hInstance, hPrevInstance, lpCmdLine, nShowCmd, *rend);

	RenderWindow::WindowWaitLoop();

	return 0;
}