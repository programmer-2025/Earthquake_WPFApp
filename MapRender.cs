using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Media;
using System.Windows.Shapes;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Brush = System.Drawing.Brush;
using Color = System.Drawing.Color;
using Geometry = NetTopologySuite.Geometries.Geometry;
using Pen = System.Drawing.Pen;
using Polygon = NetTopologySuite.Geometries.Polygon;


namespace Earthquake_WPFApp {

    /// <summary>
    /// マップを描画するためのクラス
    /// </summary>
    internal class MapRender {

        private readonly Bitmap bitmap;  // 出力する画像データ

        private readonly ShapefileDataReader reader;    //  ライブラリ側のシェイプデータを保持クラス
        private Envelope envelope;                      // 　大きいボックス的なもの（※描画範囲を決定するのに使用する）
        private readonly List<IFeature> featureList;    //  属性と図形のリスト
        private readonly List<Action<Graphics>> actionList; //　最終的に実行するアクションのリスト


        public MapRender(string path, int width, int height) {
            this.bitmap = new Bitmap(width, height);
            this.reader = new ShapefileDataReader(path, GeometryFactory.Default, Encoding.UTF8);
            this.featureList = [];
            this.actionList = [];
            this.envelope = new Envelope();

            while (reader.Read()) { // ファイルから読む
                var geometry = reader.Geometry;
                var attributes = new AttributesTable(); //　属性

                for (int i = 0; i < reader.DbaseHeader.NumFields; i++) {
                    string name = reader.DbaseHeader.Fields[i].Name; //フィールド名
                    object value = reader.GetValue(i + 1); //値

                    attributes.Add(name, value);
                }
                featureList.Add(new Feature(geometry, attributes)); // ファイルから読み取ったFeatureをリストに入れる
            }

            foreach (var feature in featureList) {
                this.envelope.ExpandToInclude(feature.Geometry.EnvelopeInternal);
            }
        }

        /// <summary>
        /// 面を塗る関数。
        /// </summary>
        /// <param name="color">色</param>
        /// <param name="filterFunc">塗る範囲</param>
        public MapRender SetFill(Color color, Func<IFeature, bool> filterFunc) {
            var brush = new SolidBrush(color);
            actionList.Add(graphics => {        
                foreach (var filter in featureList.Where(filterFunc)) {
                    DrawGeometry(graphics, null, brush, filter.Geometry);
                }
            });
            return this;
        }

        /// <summary>
        /// 線を引く関数
        /// </summary>
        /// <param name="color">色</param>
        /// <param name="filterFunc">塗る範囲</param>
        public MapRender AddLine(Color color, int strokeSize, Func<IFeature, bool> filterFunc) {
            var pen = new Pen(color, strokeSize);
            actionList.Add(graphics => {        
                foreach (var filter in featureList.Where(filterFunc))　{
                    DrawGeometry(graphics, pen, null, filter.Geometry);
                }
            });
            return this;
        }

        private void DrawGeometry(Graphics graphics, Pen pen, Brush brush, Geometry geometry) {
            if (geometry is Polygon polygon) DrawPolygon(graphics, pen, brush, polygon);
            else if (geometry is MultiPolygon multiPolygon)
            {
                foreach (Polygon p in multiPolygon.Geometries)
                    DrawPolygon(graphics, pen, brush, p);
            }
        }

        /// <summary>
        /// ポリゴンを描画する関数
        /// </summary>
        private void DrawPolygon(Graphics graphics, Pen pen, Brush brush, Polygon polygon)
        {
            using var path = new GraphicsPath();
            path.FillMode = FillMode.Winding;
            path.AddPolygon(ToPoints(polygon.ExteriorRing.Coordinates));

            foreach (var hole in polygon.InteriorRings)
            {
                path.AddPolygon(ToPoints(hole.Coordinates));
            }
        
            if (brush != null) graphics.FillPath(brush, path);
            if (pen != null) graphics.DrawPolygon(pen, ToPoints(polygon.ExteriorRing.Coordinates));
        }

        /// <summary>
        /// 描画範囲を設定する関数
        /// </summary>
        /// <returns></returns>
        public MapRender SetDrawArea(Func<IFeature, bool> filter) {
            List<Envelope>? envelopes = featureList.Where(filter)
                .Select(feature => feature.Geometry.EnvelopeInternal)
                .ToList();  // IFeatureのリストから、Envelopeを取得し、Envelopeのリストにする

            if (envelopes.Count == 0) {
                // 見つからない場合は新しいインスタンスを返す
                this.envelope = new Envelope();
                return this;
            }

            var result = new Envelope(envelopes.First());

            foreach (var env in envelopes) result.ExpandToInclude(env);
            this.envelope = result;

            return this;
        }

        /// <summary>
        /// 画像にする関数
        /// </summary>
        /// <returns></returns>
        public Bitmap Build() {
            using (var graphic = Graphics.FromImage(bitmap)) {
                graphic.SmoothingMode = SmoothingMode.AntiAlias;    //アンチエイジングを有効
                foreach (var action in actionList) action(graphic); // リストにたまったアクションを実行する
            }
            return bitmap;
        }

        /// <summary>
        /// Coordinate[]をPointF[]に変換する関数
        /// </summary>
        private PointF[] ToPoints(Coordinate[] coordinates) {
            return coordinates.Select(coordinate => {
                float x = (float)((coordinate.X - envelope.MinX) * bitmap.Width / envelope.Width);
                float y = (float)((envelope.MaxY - coordinate.Y) * bitmap.Height / envelope.Height);

                return new PointF(x, y);
            }).ToArray();
        }
    }
}
