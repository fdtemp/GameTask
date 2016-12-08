#pragma once
#include "4.h"
#include <list>
#include <queue>
#include "include\armadillo"
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
	class AxisAngle {
	public:
		float angle;
		Vector3 vector;
		AxisAngle() :vector(0,0,1), angle(0) {}
		AxisAngle(Vector3 _vector, float _angle) :vector(_vector), angle(_angle) {}
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
		Transform() :Transform(Vector3(), AxisAngle(), Vector3(1, 1, 1)) {}
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
		Premitive(Vector3 _a, Vector3 _b, Vector3 _c) :a(_a), b(_b), c(_c) {}
	};
	class Model {
	public:
		Transform *transform;
		Model *Parent;
		virtual list<Premitive> GetPremitive() {
			return list<Premitive>();
		};
	};
	class Cube : Model {
	public:
		Vector3 Size;
		list<Premitive> GetPremitive() {
			list<Premitive> lis = list<Premitive>();
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
			AspectRatio = 4 / 3;
			Near = 1;
			Far=100;
			transform = new Transform();
		}
		~Camera() {
			delete transform;
		}
	};
	class Renderer {
	private:
		list<Model*> ModelList;
		Camera *mCamera;
		Model *CurrentModel;
		list<Premitive> PremitiveList;
		Premitive CurrentPremitive;
		mat ModelMat, ViewMat, ProjectionMat, transformMat;

		void StartRender() {
			for (int i = 0;i < ScreenWidth;i++)
				for (int j = 0;j < ScreenHeight;j++)
					Output[i][j] = Color();
			//Get View Matrix
			mCamera->transform->Scale = Vector3(1, 1, 1);
			mCamera->transform->Translation.Reverse();
			mCamera->transform->Rotation.Reverse();
			ViewMat = mCamera->transform->GetTransformMat();
			mCamera->transform->Translation.Reverse();
			mCamera->transform->Rotation.Reverse();
			//Get Projection Matrix
			float fax = 1.0f / tan(mCamera->theta * 0.5f);
			ProjectionMat = mat(4, 4);
			ProjectionMat(0, 0) = fax / mCamera->AspectRatio;
			ProjectionMat(1, 1) = fax;
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
			PremitiveList = CurrentModel->GetPremitive();
		}
		void TransformPremitive() {
			
		}
		void Raster() {

		}
		void Shade() {

		}
		void EndRender() {

		}
	public:

		Color Output[ScreenWidth][ScreenHeight];

		Renderer() {
			mCamera = new Camera();
		}
		void Refresh() {
			StartRender();
			for (list<Model*>::iterator now = ModelList.begin();now != ModelList.end();++now) {
				CurrentModel = *now;
				StartModel();
				for (list<Premitive>::iterator p = PremitiveList.begin();p != PremitiveList.end();++p) {
					CurrentPremitive = *p;
					TransformPremitive();
					Raster();
					Shade();
				}
			}
			EndRender();
		}
	};
}
