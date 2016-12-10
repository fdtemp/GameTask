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
		mat GetTranslationMat() {
			mat m(4,4);
			m.fill(0);
			m(0, 0) = m(1, 1) = m(2, 2) = m(3, 3) = 1;
			m(3, 0) = Translation.x;
			m(3, 1) = Translation.y;
			m(3, 2) = Translation.z;
			return m;
		}
		mat GetScaleMat() {
			mat m(4,4);
			m.fill(0);
			m(0, 0) = Scale.x;
			m(1, 1) = Scale.y;
			m(2, 2) = Scale.z;
			m(3, 3) = 1;
			return m;
		}
		mat GetRotationMat() {
			float s = sin(Rotation.angle/2);
			Vector3 normal = Rotation.vector.normalise();
			float
				x = Rotation.vector.x*s,
				y = Rotation.vector.y*s,
				z = Rotation.vector.z*s,
				w = cos(Rotation.angle/2);
			mat m(4, 4);
			m   << 1 - 2 * (y*y + z*z) << 2 * x*y - 2 * w*z   << 2 * w*y + 2 * x*z   << 0 << endr
			    << 2 * x*y + 2 * w*z   << 1 - 2 * (x*x + z*z) << -2 * w*x + 2 * y*z  << 0 << endr
			    << -2 * w*y + 2 * x*z  << 2 * w*x + 2 * y*z   << 1 - 2 * (x*x + y*y) << 0 << endr
			    << 0                   << 0                   << 0                   << 1 << endr;
			return m;
		}

		Vector3 Translation, Scale;
		AxisAngle Rotation;
		mat GetTransformMat() { return GetScaleMat() * GetRotationMat() * GetTranslationMat(); }
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
		float au, av, bu, bv, cu, cv;
		Premitive() :a(), b(), c() {}
		Premitive(Vector3 &_a, Vector3 &_b, Vector3 &_c, float _au, float _av, float _bu, float _bv, float _cu, float _cv)
			:a(_a), b(_b), c(_c), au(_au), av(_av), bu(_bu), bv(_bv), cu(_cu), cv(_cv) {}
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
			lis.push_back(Premitive(vec[0], vec[1], vec[2], 0, 0, 1, 0, 0, 1));
			lis.push_back(Premitive(vec[1], vec[2], vec[3], 1, 0, 0, 1, 1, 1));
			lis.push_back(Premitive(vec[0], vec[1], vec[4], 0, 0, 0, 1, 1, 0));
			lis.push_back(Premitive(vec[1], vec[4], vec[5], 0, 1, 1, 0, 1, 1));
			lis.push_back(Premitive(vec[0], vec[2], vec[6], 0, 0, 0, 1, 1, 1));
			lis.push_back(Premitive(vec[0], vec[4], vec[6], 0, 0, 1, 0, 1, 1));
			lis.push_back(Premitive(vec[2], vec[3], vec[7], 0, 0, 0, 1, 1, 1));
			lis.push_back(Premitive(vec[2], vec[6], vec[7], 0, 0, 1, 0, 1, 1));
			lis.push_back(Premitive(vec[1], vec[3], vec[5], 0, 0, 0, 1, 1, 0));
			lis.push_back(Premitive(vec[3], vec[5], vec[7], 0, 1, 1, 0, 1, 1));
			lis.push_back(Premitive(vec[4], vec[5], vec[6], 0, 0, 0, 1, 1, 0));
			lis.push_back(Premitive(vec[5], vec[6], vec[7], 0, 1, 1, 0, 1, 1));
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
			transform->Rotation.angle = 3.1415926f / 4;
		}
		~Camera() {
			delete transform;
		}
	};
	class Light {
	public:
		Vector3 Direction;
		Light() {
			Direction = Vector3(1, -1, -1);
		}
	};
	class ScreenPoint {
	public:
		int x, y;
		float z;
		ScreenPoint() :x(0), y(0) {}
		ScreenPoint(int _x, int _y, float _z) :x(_x), y(_y), z(_z) {}
	};
	class ScreenLine {
	public:
		ScreenPoint Start, End;
		ScreenLine(ScreenPoint &s, ScreenPoint &e) :Start(s), End(e) {}
	};
	class ZBufferPoint {
	public:
		float z;
		Vector3 worldpoint;
		Vector3 light;
		Vector3 view;
		float u, v;
		ZBufferPoint() :z(-1e20), worldpoint(), light(), view(), u(0), v(0) {}
	};
	class Renderer {
	public:
		Color Map[10][10];
		Color Output[ScreenWidth][ScreenHeight];
		ZBufferPoint ZBuffer[ScreenWidth][ScreenHeight];
		list<Model*> ModelList;
		Camera *mCamera;
		Light *mLight;
		Model *CurrentModel;
		mat ModelMat, ViewMat, ProjectionMat, TransformMat;
		list<Premitive> PremitiveList;
		Premitive *CurrentPremitive;
		ScreenPoint sp1, sp2, sp3;
		list<ScreenLine> LineList;

		/*mat ModelTransform(Vector3 &v) {
			mat in(1, 4);
			in << v.x << v.y << v.z << 1 << endr;
			return in * ModelMat;
		}
		mat ViewTransform(mat &m) {
			return m * ViewMat;
		}
		mat ProjectionTransform(mat &m) {
			return m * ProjectionMat;
		}*/

		void DrawLine(ScreenPoint &a, ScreenPoint &b) {
			LineList.push_back(ScreenLine(a, b));
		}
		void DrawTriangle_Wireframe(ScreenPoint &a, ScreenPoint &b, ScreenPoint &c) {
			DrawLine(a, b);
			DrawLine(b, c);
			DrawLine(c, a);
		}
		Color GetColorByUV(float u, float v) {
			return Map[(int)floor(u * 10)][(int)floor(v * 10)];
		}
		void AddZBuffer(int x, int y) {
			if (x<0 || x>ScreenWidth || y<0 || y>ScreenHeight) return;
			float
				fx = (float)x + 0.5,
				fy = (float)y + 0.5;
			float
				c1 = ((sp2.y - sp3.y)*(fx - sp3.x) - (sp2.x - sp3.x)*(fy - sp3.y))
				/ ((sp2.y - sp3.y)*(sp1.x - sp3.x) - (sp2.x - sp3.x)*(sp1.y - sp3.y)),
				c2 = ((sp1.y - sp3.y)*(fx - sp3.x) - (sp1.x - sp3.x)*(fy - sp3.y))
				/ ((sp1.y - sp3.y)*(sp2.x - sp3.x) - (sp1.x - sp3.x)*(sp2.y - sp3.y));
			float
				fz = 1/(c1 / sp1.z + c2 / sp2.z + (1 - c1 - c2) / sp3.z);
			ZBufferPoint &bp = ZBuffer[x][y];
			if (bp.z > fz) return;
			else bp.z = fz;
			bp.u = fz * (
				(CurrentPremitive->au * c1) / sp1.z
				+ (CurrentPremitive->bu * c2) / sp2.z
				+ (CurrentPremitive->cu * (1 - c1 - c2)) / sp3.z
			);
			bp.v = fz * (
				(CurrentPremitive->av * c1) / sp1.z
				+ (CurrentPremitive->bv * c2) / sp2.z
				+ (CurrentPremitive->cv * (1 - c1 - c2)) / sp3.z
			);
			Output[x][y] = GetColorByUV(bp.u,bp.v);
		}
		void RasterizeTriangle(ScreenPoint &top, ScreenPoint &left, ScreenPoint &right) {
			for (int y = left.y;y <= top.y;++y) {
				float
					Rate = (float)(y - left.y) / (top.y - left.y);
				int
					Start = left.x + (top.x - left.x) * Rate + 0.5,
					End = right.x + (top.x - right.x) * Rate + 0.5;
				for (int i = Start;i <= End;++i)
					AddZBuffer(i, y);
			}
			//DrawTriangle_Wireframe(top, left, right);
		}
		void RasterizeInverseTriangle(ScreenPoint &bot, ScreenPoint &left, ScreenPoint &right) {
			for (int y = bot.y;y <= left.y;++y) {
				float
					Rate = (float)(left.y - y) / (left.y - bot.y);
				int
					Start = left.x + (bot.x - left.x) * Rate + 0.5,
					End = right.x + (bot.x - right.x) * Rate + 0.5;
				for (int i = Start;i <= End;++i)
					AddZBuffer(i, y);
			}
			//DrawTriangle_Wireframe(bot, left, right);
		}

		void StartRender() {
			for (int i = 0;i < ScreenWidth;i++)
				for (int j = 0;j < ScreenHeight;j++) {
					ZBuffer[i][j] = ZBufferPoint();
					Output[i][j] = Color(255,255,255);
				}
			LineList.clear();
			//Get View Matrix
			mCamera->transform->Scale = Vector3(1, 1, 1);
			mCamera->transform->Translation.Reverse();
			mCamera->transform->Rotation.Reverse();
			ViewMat = mCamera->transform->GetTranslationMat() * mCamera->transform->GetRotationMat();
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
			float cot = 1 / tan(mCamera->theta / 2);
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
			mat in(1, 4);
			in << CurrentPremitive->a.x << CurrentPremitive->a.y << CurrentPremitive->a.z << 1 << endr;
			mat out = in * TransformMat;
			sp1 = ScreenPoint(
				floor(ScreenWidth * (1 + out(0, 0) / out(0, 3)) / 2),
				floor(ScreenWidth*(1 + out(0, 1) / out(0, 3)) / 2),
				out(0, 3)
			);
			in << CurrentPremitive->b.x << CurrentPremitive->b.y << CurrentPremitive->b.z << 1 << endr;
			out = in * TransformMat;
			sp2 = ScreenPoint(
				floor(ScreenWidth * (1 + out(0, 0) / out(0, 3)) / 2),
				floor(ScreenWidth*(1 + out(0, 1) / out(0, 3)) / 2),
				out(0, 3)
			);
			in << CurrentPremitive->c.x << CurrentPremitive->c.y << CurrentPremitive->c.z << 1 << endr;
			out = in * TransformMat;
			sp3 = ScreenPoint(
				floor(ScreenWidth * (1 + out(0, 0) / out(0, 3)) / 2),
				floor(ScreenWidth*(1 + out(0, 1) / out(0, 3)) / 2),
				out(0, 3)
			);
			//Draw Wireframe
			DrawTriangle_Wireframe(sp1, sp2, sp3);
		}
		void Rasterize() {
			if (sp1.y == sp2.y && sp2.y == sp3.y) return;
			if (sp1.y == sp2.y) {
				ScreenPoint *l, *r;
				if (sp1.x < sp2.x) {
					l = &sp1;r = &sp2;
				} else if (sp2.x < sp1.x) {
					l = &sp2, r = &sp1;
				} else {
					return;
				}
				if (sp1.y < sp3.y) {
					RasterizeTriangle(sp3, *l, *r);
				} else {
					RasterizeInverseTriangle(sp3, *l, *r);
				}
			} else if (sp1.y == sp3.y) {
				ScreenPoint *l, *r;
				if (sp1.x < sp3.x) {
					l = &sp1;r = &sp3;
				} else if (sp3.x < sp1.x) {
					l = &sp3, r = &sp1;
				} else {
					return;
				}
				if (sp1.y < sp2.y) {
					RasterizeTriangle(sp2, *l, *r);
				} else {
					RasterizeInverseTriangle(sp2, *l, *r);
				}
			} else if (sp2.y == sp3.y) {
				ScreenPoint *l, *r;
				if (sp2.x < sp3.x) {
					l = &sp2;r = &sp3;
				} else if (sp3.x < sp2.x) {
					l = &sp3, r = &sp2;
				} else {
					return;
				}
				if (sp2.y < sp1.y) {
					RasterizeTriangle(sp1, *l, *r);
				} else {
					RasterizeInverseTriangle(sp1, *l, *r);
				}
			} else {
				ScreenPoint *top, *mid, *bot;
				if (sp1.y < sp2.y) {
					if (sp2.y < sp3.y) {
						top = &sp3;mid = &sp2;bot = &sp1;
					} else if (sp1.y < sp3.y) {
						top = &sp2;mid = &sp3;bot = &sp1;
					} else {
						top = &sp2;mid = &sp1;bot = &sp3;
					}
				} else {
					if (sp1.y < sp3.y) {
						top = &sp3;mid = &sp1;bot = &sp2;
					} else if (sp2.y < sp3.y) {
						top = &sp1;mid = &sp3;bot = &sp2;
					} else {
						top = &sp1;mid = &sp2;bot = &sp3;
					}
				}
				float rate = (float)(mid->y - bot->y) / (top->y - bot->y);
				ScreenPoint ap = ScreenPoint(
					bot->x + (top->x - bot->x) * rate + 0.5,
					mid->y,
					top->z * rate + bot->z *(1 - rate)
				);
				if ( mid->x < ap.x) {
					RasterizeTriangle(*top, *mid, ap);
					RasterizeInverseTriangle(*bot, *mid, ap);
				} else if (ap.x < mid->x) {
					RasterizeTriangle(*top, ap, *mid);
					RasterizeInverseTriangle(*bot, ap, *mid);
				}
			}
		}
		void Shade() {

		}
		void EndRender() {

		}
		
		Model *cube1 = new Cube(), *cube2 = new Cube(), *cube3 = new Cube();
		Renderer() {
			Color c[8];
			c[0] = Color(0, 0, 0);
			c[1] = Color(255, 0, 0);
			c[2] = Color(0, 255, 0);
			c[3] = Color(0, 0, 255);
			c[4] = Color(255, 255, 0);
			c[5] = Color(255, 0, 255);
			c[6] = Color(0, 255, 255);
			c[7] = Color(128, 128, 128);
			for (int i = 0;i < 10;i++)
				for (int j = 0;j < 10;j++)
					Map[i][j] = c[((10007*i*j + 100007)%1007) % 8];
			LineList = list<ScreenLine>();
			mCamera = new Camera();
			mLight = new Light();
			cube1->transform->Translation = Vector3(3, -3.5, -0.5);
			cube1->transform->Rotation.angle = -3.1415926f / 4;
				cube2->Parent = cube1;
				cube3->Parent = cube1;
				cube2->transform->Translation = Vector3(1, 1, 1);
				cube3->transform->Translation = Vector3(-1, -1, 1);
			ModelList.push_back(cube1);
			ModelList.push_back(cube2);
			ModelList.push_back(cube3);
		}
		void Refresh() {
			cube1->transform->Rotation.angle += -3.1415926f / 12;
			cube2->transform->Rotation.angle += -3.1415926f / 12;
			cube3->transform->Rotation.angle += -3.1415926f / 12;
			StartRender();
			for (list<Model*>::iterator now = ModelList.begin();now != ModelList.end();++now) {
				CurrentModel = *now;
				StartModel();
				/*ModelMat.print("Model Matrix :");
				ViewMat.print("View Matrix :");
				ProjectionMat.print("Projection Matrix :");
				TransformMat.print("Transform Matrix :");*/
				PremitiveList = CurrentModel->GetPremitive();
				for (list<Premitive>::iterator i = PremitiveList.begin();i != PremitiveList.end();++i) {
					CurrentPremitive = &(*i);
					TransformPremitive();
					Rasterize();
					Shade();
				}
				PremitiveList.clear();
			}
			EndRender();
		}
	};
}
