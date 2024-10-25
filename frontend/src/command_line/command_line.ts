export class CommandLine {
 private container: HTMLElement;
 private input: HTMLInputElement;
 private outputContainer: HTMLElement;
 private onCommand: (command: string) => string;
 private static readonly template = `
    <div class="cli-container">
      <style>
        .cli-container {
          background-color: rgba(30, 30, 30, 0.8);
          backdrop-filter: blur(8px);
          -webkit-backdrop-filter: blur(12px);
          color: #f0f0f0;
          font-family: 'Courier New', monospace;
          padding: 20px;
          height: 250px;
          width: 600px;
          overflow-y: auto;
          position: fixed;
          bottom: 20px;
          left: 50%;
          transform: translateX(-50%);
          border-radius: 12px;
          display: flex;
          flex-direction: column;
        }
        
        .cli-container::-webkit-scrollbar {
          width: 8px;
        }
        
        .cli-container::-webkit-scrollbar-track {
          background: rgba(30, 30, 30, 0.3);
          border-radius: 4px;
        }
        
        .cli-container::-webkit-scrollbar-thumb {
          background: rgba(80, 250, 123, 0.3);
          border-radius: 4px;
        }
        
        .cli-container::-webkit-scrollbar-thumb:hover {
          background: rgba(80, 250, 123, 0.5);
        }
       
        .cli-output {
          margin: 0;
          padding: 2px 0;
          white-space: pre-wrap;
        }
       
        .cli-input-line {
          display: flex;
          align-items: center;
          margin-top: 8px;
        }
       
        .cli-prompt {
          color: #50fa7b;
          margin-right: 8px;
        }
       
        .cli-input {
          background: transparent;
          border: none;
          color: #f0f0f0;
          font-family: inherit;
          font-size: inherit;
          flex-grow: 1;
          outline: none;
        }
      </style>
      <p class="cli-output">Welcome to Keep Lord: Warriors</p>
      <p class="cli-output">Type 'help' for available commands</p>
      <div class="cli-input-line">
        <span class="cli-prompt">$</span>
        <input type="text" class="cli-input" autofocus>
      </div>
    </div>
  `;

 constructor(onCommand: (command: string) => string) {
  this.onCommand = onCommand;
  const template = document.createElement("template");
  template.innerHTML = CommandLine.template.trim();
  const content = template.content.querySelector(".cli-container");
  if (!content) {
   throw new Error("CLI container not found in template");
  }
  this.container = content as HTMLElement;
  this.input = this.container.querySelector(".cli-input") as HTMLInputElement;
  this.outputContainer = this.container.querySelector(
   ".cli-output"
  ) as HTMLElement;
  const mountElement = document.querySelector("body");
  if (!mountElement) {
   throw new Error(`Mount point "body" not found`);
  }
  mountElement.appendChild(this.container);
  this.setupEventListeners();
 }

 private setupEventListeners() {
  this.input.addEventListener("keydown", (event) => {
   if (event.key === "Enter") {
    const command = this.input.value.trim();
    if (command) {
     this.handleCommand(command);
     this.input.value = "";
    }
   }
  });
  this.container.addEventListener("click", () => {
   this.input.focus();
  });
 }

 private handleCommand(command: string) {
  this.addOutput(`$ ${command}`);
  this.addOutput(this.onCommand(command));
  this.scrollToBottom();
 }

 private scrollToBottom() {
  this.container.scrollTo({
   top: this.container.scrollHeight,
  });
 }

 private addOutput(text: string) {
  const output = document.createElement("p");
  output.className = "cli-output";
  output.textContent = text;
  this.container.insertBefore(output, this.input.parentElement);
 }

 public focus() {
  this.input.focus();
 }

 public clear() {
  const outputs = this.container.querySelectorAll(".cli-output");
  outputs.forEach((output) => output.remove());
 }
}
