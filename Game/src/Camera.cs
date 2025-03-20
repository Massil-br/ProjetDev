using SFML.Graphics;
using SFML.System;

namespace src
{
    public class Camera
    {
        private View view;

        public Camera(float width, float height)
        {
            view = new View(new FloatRect(0, 0, width, height));
        }

        public void Update(Vector2f targetPosition)
        {
            view.Center = targetPosition;
        }
        public void Resize(float width, float height){
            view.Size = new Vector2f(width ,height);
        }

        public float GetWidth(){
            return view.Size.X;
        }
        public float GetHeight(){
            return view.Size.Y;
        }

        public View GetView()
        {
            return view;
        }
    }
}