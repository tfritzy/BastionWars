CS_FILE="Schema.cs"

echo "Generating c#"
npx protoc -I=. --csharp_out="." schema.proto

if [ -f "$CS_FILE" ]; then
  sed -i 's/1591, 0612, 3021/1591, 0612, 3021, 8600, 8981/g' "$CS_FILE"
  echo "Replaced text in $CS_FILE"
else
  echo "Warning: $CS_FILE not found. Skipping text replacement."
fi

echo "Generating js"
npx pbjs schema.proto --ts "../frontend/src/Schema.ts" 
