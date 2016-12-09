#pragma once
#include "4.h"
#include <list>
#include <armadillo>
using namespace std;
using namespace arma;
namespace Render {
	class Color {
	public:
		byte r, g, b;
		Color() :r(0), g(0), b(0) {}
		Color(byte _r, byte _g, byte _b) :r(_r), g(_g), b(_b) {}
	};
	class Vector3 {
	public:
		float x, y, z;
		Vector3() :x(0), y(0), z(0) {}
		Vector3(float _x, float _y, float _z) :x(_x), y(_y), z(_z) {}
		Vector3 normalise() {
			float len = x*x + y*y + z*z;
			return Vector3(x / len, y / len, z / len);
		}
		void Reverse() { x = -x;y = -y;z = -z; }
	};
	class Vector4 {
	public:
		float x, y, z, w;
		Vector4(float _x, float _y, float _z, float _w) :x(_x), y(_y), z(_z), w(_w) {}
	};
	class AxisAngle {
	public:
		float angle;
		Vector3 vector;
		AxisAngle() :vector(0,0,1), angle(0) {}
		AxisAngle(Vector3 &_vector, float _angle) :vector(_vector), angle(_angle) {}
		void Reverse() { angle = -angle; }
	};
	class Transform {
	public:
		mat GetTranslationMat(Vector3 &Translation) {
			mat m(4,4);
			m.fill(0);
			m(0, 0) = m(1, 1) = m(2, 2) = m(3, 3) = 1;
			m(3, 0) = Translation.x;
			m(3, 1) = Translation.y;
			m(3, 2) = Translation.z;
			return m;
		}
		mat GetScaleMat(Vector3 &Scale) {
			mat m(4,4);
			m.fill(0);
			m(0, 0) = Scale.x;
			m(1, 1) = Scale.y;
			m(2, 2) = Scale.z;
			m(3, 3) = 1;
			return m;
		}
		mat GetRotationMat(AxisAngle &AxisAngle) {
			float s = sin(AxisAngle.angle/2);
			Vector3 normal = AxisAngle.vector.normalise();
			float
				x = AxisAngle.vector.x*s,
				y = AxisAngle.vector.y*s,
				z = AxisAngle.vector.z*s,
				w = cos(AxisAngle.angle/2);
			mat m(4, 4);
			m   << 1 - 2 * (y*y + z*z) << 2 * x*y - 2 * w*z   << 2 * w*y + 2 * x*z   << 0 << endr
			    << 2 * x*y + 2 * w*z   << 1 - 2 * (x*x + z*z) << -2 * w*x + 2 * y*z  << 0 << endr
			    << -2 * w*y + 2 * x*z  << 2 * w*x + 2 * y*z   << 1 - 2 * (x*x + y*y) << 0 << endr
			    << 0                   << 0                   << 0                   << 1 << endr;
			return m;
		}

		Vector3 Translation, Scale;
		AxisAngle Rotation;
		mat GetTransformMat() { return GetScaleMat(Scale) * GetRotationMat(Rotation) * GetTranslationMat(Translation); }
		Transform() :Transform(Vector3(0,0,0), AxisAngle(Vector3(0,0,1),0), Vector3(1, 1, 1)) {}
		Transform(Vector3 translation, AxisAngle axisAngle, Vector3 scale) {
			Translation = translation;
			Rotation = axisAngle;
			Scale = scale;
		}
	};
	class Premitive {
	public:
		Vector3 a, b, c;
		Premitive() :a(), b(), c() {}
		Premitive(Vector3 &_a, Vector3 &_b, Vector3 &_c) :a(_a), b(_b), c(_c) {}
	};
	class Model {
	public:
		Transform *transform;
		Model *Parent;
		virtual list<Premitive> GetPremitive() { return list<Premitive>(); };
	};
	class Cube : public Model {
	public:
		Vector3 Size;
		list<Premitive> GetPremitive() {
			Vector3 vec[8] = {
				Vector3(0,0,0),
				Vector3(Size.x,0,0),
				Vector3(0,Size.y,0),
				Vector3(Size.x,Size.y,0),
				Vector3(0,0,Size.z),
				Vector3(Size.x,0,Size.z),
				Vector3(0,Size.y,Size.z),
				Vector3(Size.x,Size.y,Size.z),
			};
			list<Premitive> lis = list<Premitive>();
			lis.push_back(Premitive(vec[0], vec[1], vec[2]));
			lis.push_back(Premitive(vec[1], vec[2], vec[3]));
			lis.push_back(Premitive(vec[0], vec[1], vec[4]));
			lis.push_back(Premitive(vec[1], vec[4], vec[5]));
			lis.push_back(Premitive(vec[0], vec[2], vec[6]));
			lis.push_back(Premitive(vec[0], vec[4], vec[6]));
			lis.push_back(Premitive(vec[2], vec[3], vec[7]));
			lis.push_back(Premitive(vec[2], vec[6], vec[7]));
			lis.push_back(Premitive(vec[1], vec[3], vec[5]));
			lis.push_back(Premitive(vec[3], vec[5], vec[7]));
			lis.push_back(Premitive(vec[4], vec[5], vec[6]));
			lis.push_back(Premitive(vec[5], vec[6], vec[7]));
			return lis;
		};
		Cube() {
			Size = Vector3(1, 1, 1);
			transform = new Transform();
			Parent = NULL;
		}
		~Cube() {
			delete transform;
		}
	};
	class Camera {
	public:
		float theta, AspectRatio,Near,Far;
		Transform *transform;
		Camera() {
			theta = 3.1415926f / 2;
			AspectRatio = ScreenWidth / ScreenHeight;
			Near = 1;
			Far=1000;
			transform = new Transform();
			//transform->Rotation.angle = 3.1415926f / 4;
		}
		~Camera() {
			delete transform;
		}
	};
	class Line {
	public:
		Vector3 Start, End;
		Line(Vector3 &s, Vector3 &e) :Start(s), End(e) {}
	};
	class Renderer {
	public:
		list<Model*> ModelList;
		Camera *mCamera;
		Model *CurrentModel;
		list<Premitive> PremitiveList;
		Premitive *CurrentPremitive;
		mat ModelMat, ViewMat, ProjectionMat, TransformMat;
		list<Line> LineList;
		Vector3 Reflect(Vector3 &v) {
			mat in(1, 4);
			in << v.x << v.y << v.z << 1 << endr;
			mat out = in * TransformMat;
			float w = out(0, 3);
			/*cout << out(0, 0) << "," << out(0, 1) << "," << out(0, 2) << "," << w << endl;
			cout << (1 + out(0, 0) / w) / 2 << "," << (1 + out(0, 1)) / 2 << "," << out(0, 2)<<endl;*/
			return Vector3((1 + out(0, 0) / w) / 2, (1 + out(0, 1) / w) / 2, out(0, 2));
		}
		mat ModelTransform(Vector3 &v) {
			mat in(1, 4);
			in << v.x << v.y << v.z << 1 << endr;
			return in * ModelMat;
		}
		mat ViewTransform(mat &m) {
			return m * ViewMat;
		}
		mat ProjectionTransform(mat &m) {
			return m * ProjectionMat;
		}

		void DrawLine(Render::Vector3 &a, Render::Vector3 &b) {
			LineList.push_back(Line(a, b));
		}
		void DrawTriangle(Render::Vector3 &a, Render::Vector3 &b, Render::Vector3 &c) {
			DrawLine(a, b);
			DrawLine(b, c);
			DrawLine(c, a);
		}

		void StartRender() {
			for (int i = 0;i < ScreenWidth;i++)
				for (int j = 0;j < ScreenHeight;j++)
					Output[i][j] = Color(255,255,255);
			LineList.clear();
			//Get View Matrix
			mCamera->transform->Scale = Vector3(1, 1, 1);
			mCamera->transform->Translation.Reverse();
			mCamera->transform->Rotation.Reverse();
			ViewMat = mCamera->transform->GetTransformMat();
			mCamera->transform->Translation.Reverse();
			mCamera->transform->Rotation.Reverse();
			mat x2z(4, 4);
			x2z.fill(0);
			x2z(0, 2) = 1;
			x2z(1, 0) = 1;
			x2z(2, 1) = 1;
			x2z(3, 3) = 1;
			ViewMat = ViewMat * x2z;
			//Get Projection Matrix
			float cot = 1.0f / tan(mCamera->theta / 2);
			ProjectionMat = mat(4, 4);
			ProjectionMat.fill(0);
			ProjectionMat(0, 0) = cot / mCamera->AspectRatio;
			ProjectionMat(1, 1) = cot;
			ProjectionMat(2, 2) = mCamera->Far / (mCamera->Far - mCamera->Near);
			ProjectionMat(2, 3) = -mCamera->Near * mCamera->Far / (mCamera->Far - mCamera->Near);
			ProjectionMat(3, 2) = 1;
		}
		void StartModel() {
			//Get Model Matrix
			ModelMat = CurrentModel->transform->GetTransformMat();
			Model *t = CurrentModel;
			while (t->Parent != NULL) {
				t = t->Parent;
				ModelMat *= t->transform->GetTransformMat();
			}
			//Get Transform Matrix
			TransformMat = ModelMat * ViewMat * ProjectionMat;
		}
		void TransformPremitive() {
			Vector3 a, b, c;
			a = Reflect(CurrentPremitive->a);
			b = Reflect(CurrentPremitive->b);
			c = Reflect(CurrentPremitive->c);
			DrawTriangle(a, b, c);
			/*mat m = ModelTransform(CurrentPremitive->a);
			m.print("a world :");
			m = ViewTransform(m);
			m.print("a camera :");
			m = ProjectionTransform(m);
			m.print("a final :");
			m = ModelTransform(CurrentPremitive->b);
			m.print("b world :");
			m = ViewTransform(m);
			m.print("b camera :");
			m = ProjectionTransform(m);
			m.print("b final :");
			m = ModelTransform(CurrentPremitive->c);
			m.print("c world :");
			m = ViewTransform(m);
			m.print("c camera :");
			m = ProjectionTransform(m);
			m.print("c final :");*/

		}
		void Raster() {

		}
		void Shade() {

		}
		void EndRender() {

		}

		Color Output[ScreenWidth][ScreenHeight];

		Renderer() {
			LineList = list<Line>();
			mCamera = new Camera();
			Model *cube1 = new Cube(), *cube2 = new Cube(), *cube3 = new Cube();
			cube1->transform->Translation = Vector3(4, -0.5, -0.5);
			cube1->transform->Rotation.angle = -3.1415926f / 4;
			cube2->Parent = cube1;
			cube2->transform->Translation = Vector3(1, 1, 1);
			cube3->Parent = cube1;
			cube3->transform->Translation = Vector3(-1, -1, 1);
			ModelList.push_back(cube1);
			ModelList.push_back(cube2);
			ModelList.push_back(cube3);
		}
		void Refresh() {
			StartRender();
			for (list<Model*>::iterator now = ModelList.begin();now != ModelList.end();++now) {
				CurrentModel = *now;
				StartModel();
				ModelMat.print("Model Matrix :");
				ViewMat.print("View Matrix :");
				ProjectionMat.print("Projection Matrix :");
				TransformMat.print("Transform Matrix :");
				PremitiveList = CurrentModel->GetPremitive();
				for (list<Premitive>::iterator i = PremitiveList.begin();i != PremitiveList.end();++i) {
					CurrentPremitive = &(*i);
					TransformPremitive();
					Raster();
					Shade();
				}
				PremitiveList.clear();
			}
			EndRender();
		}
	};
}
